using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SiaConsulting.Azure.WebJobs.Extensions.KeyVaultExtension.Blobs.Models
{
    public class BlobPath
    {
        private readonly string _containerName;
        private readonly string _blobName;
        private static readonly char[] _unsafeBlobNameCharacters = { '\\' };

        public BlobPath(string containerName, string blobName)
        {
            _containerName = containerName ?? throw new ArgumentNullException("containerName");
            _blobName = blobName;
        }

        public string ContainerName 
            => _containerName;

        public string BlobName 
            => _blobName;

        public override string ToString()
        {
            var result = _containerName;
            if (!string.IsNullOrEmpty(_blobName))
            {
                result += "/" + _blobName;
            }

            return result;
        }

        public static BlobPath ParseAndValidate(string? value, bool isContainerBinding = false)
        {

            if (!TryParseAndValidate(value, out var errorMessage, out var path, isContainerBinding))
            {
                throw new FormatException(errorMessage);
            }
            if(path is null)
            {
                throw new FormatException(errorMessage);
            }

            return path;
        }

        // throws exception if failed
        public static BlobPath? ParseAbsUrl(string blobUrl)
        {
            if (TryParseAbsUrl(blobUrl, out var returnV))
            {
                return returnV;
            }
            throw new FormatException($"Invalid absolute blob url: {blobUrl}");
        }

        // similar to TryParse, but take in Url
        // does not take argument isContainerBinding since Url is blob only
        public static bool TryParseAbsUrl(string? blobUrl, out BlobPath? path)
        {
            path = null;
            if (Uri.TryCreate(blobUrl, UriKind.Absolute, out var uri))
            {
                var blob = new CloudBlob(uri);
                path = new BlobPath(blob.Container.Name, blob.Name); // use storage sdk to parse url
                return true;
            }
            return false;
        }

        public static bool TryParse(string? value, bool isContainerBinding, out BlobPath? path)
        {
            path = null;

            if (value == null)
            {
                return false;
            }

            var slashIndex = value.IndexOf('/');
            if (!isContainerBinding && slashIndex <= 0)
            {
                return false;
            }

            if (slashIndex > 0 && slashIndex == value.Length - 1)
            {
                // if there is a slash present, there must be at least one character before
                // the slash and one character after the slash.
                return false;
            }

            var containerName = slashIndex > 0 ? value.Substring(0, slashIndex) : value;
            var blobName = slashIndex > 0 ? value.Substring(slashIndex + 1) : string.Empty;

            path = new BlobPath(containerName, blobName);
            return true;
        }

        private static bool TryParseAndValidate(string? value, out string? errorMessage, out BlobPath? path, bool isContainerBinding = false)
        {

            if (!isContainerBinding && TryParseAbsUrl(value, out var possiblePath))
            {
                path = possiblePath;
                errorMessage = null;
                return true;
            }


            if (!TryParse(value, isContainerBinding, out possiblePath))
            {
                errorMessage = $"Invalid blob path specified : '{value}'. Blob identifiers must be in the format 'container/blob'.";
                path = null;
                return false;
            }

            if (!IsValidContainerName(possiblePath?.ContainerName))
            {
                errorMessage = "Invalid container name: " + possiblePath?.ContainerName;
                path = null;
                return false;
            }

            // for container bindings, we allow an empty blob name/path
            if (!(isContainerBinding && string.IsNullOrEmpty(possiblePath?.BlobName)) &&
                !IsValidBlobName(possiblePath?.BlobName, out var possibleErrorMessage))
            {
                errorMessage = possibleErrorMessage;
                path = null;
                return false;
            }

            errorMessage = null;
            path = possiblePath;
            return true;
        }

        public static bool IsValidBlobName(string? blobName, out string? errorMessage)
        {
            const string UnsafeCharactersMessage =
                "The given blob name '{0}' contain illegal characters. A blob name cannot the following character(s): '\\'.";
            const string TooLongErrorMessage =
                "The given blob name '{0}' is too long. A blob name must be at least one character long and cannot be more than 1,024 characters long.";
            const string TooShortErrorMessage =
                "The given blob name '{0}' is too short. A blob name must be at least one character long and cannot be more than 1,024 characters long.";
            const string InvalidSuffixErrorMessage =
                "The given blob name '{0}' has an invalid suffix. Avoid blob names that end with a dot ('.'), a forward slash ('/'), or a sequence or combination of the two.";

            if (blobName == null)
            {
                errorMessage = string.Format(CultureInfo.CurrentCulture, TooShortErrorMessage, string.Empty);
                return false;
            }
            if (blobName.Length == 0)
            {
                errorMessage = string.Format(CultureInfo.CurrentCulture, TooShortErrorMessage, blobName);
                return false;
            }

            if (blobName.Length > 1024)
            {
                errorMessage = string.Format(CultureInfo.CurrentCulture, TooLongErrorMessage, blobName);
                return false;
            }

            if (blobName.EndsWith(".", StringComparison.OrdinalIgnoreCase) || blobName.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = string.Format(CultureInfo.CurrentCulture, InvalidSuffixErrorMessage, blobName);
                return false;
            }

            if (blobName.IndexOfAny(_unsafeBlobNameCharacters) > -1)
            {
                errorMessage = string.Format(CultureInfo.CurrentCulture, UnsafeCharactersMessage, blobName);
                return false;
            }

            errorMessage = null;
            return true;
        }

        public static bool IsValidContainerName(string? containerName)
        {
            if (containerName == null)
            {
                return false;
            }

            if (containerName.Equals("$root"))
            {
                return true;
            }

            return Regex.IsMatch(containerName, @"^[a-z0-9](([a-z0-9\-[^\-])){1,61}[a-z0-9]$");
        }
    }
}

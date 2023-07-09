using AnimeVnInfoBackend.Utilities.Constants;
using Microsoft.AspNetCore.Mvc;

namespace AnimeVnInfoBackend.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpGet("Studio/{studioName}/{studioImage}")]
        public async Task<object?> GetStudioImage(string studioName, string studioImage, CancellationToken cancellationToken)
        {
            var folderName = Path.Combine("Image", "Studio");
            var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName);
            var fullPath = Path.Combine(pathToSave, studioName, studioImage);
            if (System.IO.File.Exists(fullPath))
            {
                var file = await System.IO.File.ReadAllBytesAsync(fullPath, cancellationToken);
                return File(file, GetMimeType(studioImage));
            }
            return null;
        }

        [HttpGet("Producer/{producerName}/{producerImage}")]
        public async Task<object?> GetProducerImage(string producerName, string producerImage, CancellationToken cancellationToken)
        {
            var folderName = Path.Combine("Image", "Producer");
            var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName);
            var fullPath = Path.Combine(pathToSave, producerName, producerImage);
            if (System.IO.File.Exists(fullPath))
            {
                var file = await System.IO.File.ReadAllBytesAsync(fullPath, cancellationToken);
                return File(file, GetMimeType(producerImage));
            }
            return null;
        }

        [HttpGet("Staff/{staffName}/{staffImage}")]
        public async Task<object?> GetStaffImage(string staffName, string staffImage, CancellationToken cancellationToken)
        {
            var folderName = Path.Combine("Image", "Staff");
            var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName);
            var fullPath = Path.Combine(pathToSave, staffName, staffImage);
            if (System.IO.File.Exists(fullPath))
            {
                var file = await System.IO.File.ReadAllBytesAsync(fullPath, cancellationToken);
                return File(file, GetMimeType(staffImage));
            }
            return null;
        }

        [HttpGet("Character/{characterName}/{characterImage}")]
        public async Task<object?> GetCharacterImage(string characterName, string characterImage, CancellationToken cancellationToken)
        {
            var folderName = Path.Combine("Image", "Character");
            var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName);
            var fullPath = Path.Combine(pathToSave, characterName, characterImage);
            if (System.IO.File.Exists(fullPath))
            {
                var file = await System.IO.File.ReadAllBytesAsync(fullPath, cancellationToken);
                return File(file, GetMimeType(characterImage));
            }
            return null;
        }

        [HttpGet("Anime/{animeName}/{animeImage}")]
        public async Task<object?> GetAnimeImage(string animeName, string animeImage, CancellationToken cancellationToken)
        {
            var folderName = Path.Combine("Image", "Anime");
            var pathToSave = Path.Combine(EnvironmentConstant.IMAGE_VOLUME, folderName);
            var fullPath = Path.Combine(pathToSave, animeName, animeImage);
            if (System.IO.File.Exists(fullPath))
            {
                var file = await System.IO.File.ReadAllBytesAsync(fullPath, cancellationToken);
                return File(file, GetMimeType(animeImage));
            }
            return null;
        }

        private string GetMimeType(string fileName) => fileName.Split(".").Last() switch
        {
            "7z" => "application/x-7z-compressed",
            "avi" => "video/x-msvideo",
            "avif" => "image/avif",
            "bmp" => "image/bmp",
            "bz" => "application/x-bzip",
            "bz2" => "application/x-bzip2",
            "cda" => "application/x-cdf",
            "csv" => "text/csv",
            "doc" => "application/msword",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "epub" => "application/epub+zip",
            "gif" => "image/gif",
            "gz" => "application/gzip",
            "ico" => "image/vnd.microsoft.icon",
            "jpeg" => "image/jpeg",
            "jpg" => "image/jpeg",
            "mid" => "audio/midi",
            "midi" => "audio/x-midi",
            "mp3" => "audio/mpeg",
            "mp4" => "video/mp4",
            "mpeg" => "video/mpeg",
            "odp" => "application/vnd.oasis.opendocument.presentation",
            "ods" => "application/vnd.oasis.opendocument.spreadsheet",
            "odt" => "application/vnd.oasis.opendocument.text",
            "oga" => "audio/ogg",
            "ogv" => "video/ogg",
            "ogx" => "application/ogg",
            "opus" => "audio/opus",
            "pdf" => "application/pdf",
            "png" => "image/png",
            "ppt" => "application/vnd.ms-powerpoint",
            "pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            "rar" => "application/vnd.rar",
            "rtf" => "application/rtf",
            "svg" => "image/svg+xml",
            "tar" => "application/x-tar",
            "ts" => "video/mp2t",
            "txt" => "text/plain",
            "vsd" => "application/vnd.visio",
            "wav" => "audio/wav",
            "weba" => "audio/webm",
            "webm" => "video/webm",
            "webp" => "image/webp",
            "xls" => "application/vnd.ms-excel",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "zip" => "application/x-zip-compressed",
            _ => "text/plain"
        };
    }
}

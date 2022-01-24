using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Blocklisting;
using NzbDrone.Core.History;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Music;
using NzbDrone.Core.Parser;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.CustomFormats
{
    public interface ICustomFormatCalculationService
    {
        List<CustomFormat> ParseCustomFormat(ParsedAlbumInfo movieInfo);
        List<CustomFormat> ParseCustomFormat(TrackFile trackFile);
        List<CustomFormat> ParseCustomFormat(Blocklist blocklist);
        List<CustomFormat> ParseCustomFormat(EntityHistory history);
    }

    public class CustomFormatCalculationService : ICustomFormatCalculationService
    {
        private readonly ICustomFormatService _formatService;
        private readonly IParsingService _parsingService;
        private readonly IArtistService _artistService;

        public CustomFormatCalculationService(ICustomFormatService formatService,
                                              IParsingService parsingService,
                                              IArtistService artistService)
        {
            _formatService = formatService;
            _parsingService = parsingService;
            _artistService = artistService;
        }

        public static List<CustomFormat> ParseCustomFormat(ParsedAlbumInfo episodeInfo, List<CustomFormat> allCustomFormats)
        {
            var matches = new List<CustomFormat>();

            foreach (var customFormat in allCustomFormats)
            {
                var specificationMatches = customFormat.Specifications
                    .GroupBy(t => t.GetType())
                    .Select(g => new SpecificationMatchesGroup
                    {
                        Matches = g.ToDictionary(t => t, t => t.IsSatisfiedBy(episodeInfo))
                    })
                    .ToList();

                if (specificationMatches.All(x => x.DidMatch))
                {
                    matches.Add(customFormat);
                }
            }

            return matches;
        }

        public static List<CustomFormat> ParseCustomFormat(TrackFile trackFile, List<CustomFormat> allCustomFormats)
        {
            var sceneName = string.Empty;
            if (trackFile.SceneName.IsNotNullOrWhiteSpace())
            {
                sceneName = trackFile.SceneName;
            }
            else if (trackFile.Path.IsNotNullOrWhiteSpace())
            {
                sceneName = Path.GetFileName(trackFile.Path);
            }

            var info = new ParsedAlbumInfo
            {
                ArtistName = trackFile.Artist.Value.Name,
                ReleaseTitle  = sceneName,
                Quality = trackFile.Quality,
                ReleaseGroup = trackFile.ReleaseGroup,
                ExtraInfo = new Dictionary<string, object>
                {
                    { "Size", trackFile.Size },
                    { "Filename", Path.GetFileName(trackFile.Path) }
                }
            };

            return ParseCustomFormat(info, allCustomFormats);
        }

        public List<CustomFormat> ParseCustomFormat(ParsedAlbumInfo movieInfo)
        {
            return ParseCustomFormat(movieInfo, _formatService.All());
        }

        public List<CustomFormat> ParseCustomFormat(TrackFile episodeFile)
        {
            return ParseCustomFormat(episodeFile, _formatService.All());
        }

        public List<CustomFormat> ParseCustomFormat(Blocklist blocklist)
        {
            var movie = _artistService.GetArtist(blocklist.ArtistId);
            var parsed = Parser.Parser.ParseAlbumTitle(blocklist.SourceTitle);

            var info = new ParsedAlbumInfo
            {
                ArtistName = movie.Name,
                ReleaseTitle = parsed?.ReleaseTitle ?? blocklist.SourceTitle,
                Quality = blocklist.Quality,
                ReleaseGroup = parsed?.ReleaseGroup,
                ExtraInfo = new Dictionary<string, object>
                {
                    { "Size", blocklist.Size }
                }
            };

            return ParseCustomFormat(info);
        }

        public List<CustomFormat> ParseCustomFormat(EntityHistory history)
        {
            var artist = _artistService.GetArtist(history.ArtistId);
            var parsed = Parser.Parser.ParseAlbumTitle(history.SourceTitle);

            long.TryParse(history.Data.GetValueOrDefault("size"), out var size);

            var info = new ParsedAlbumInfo
            {
                ArtistName = artist.Name,
                ReleaseTitle = parsed?.ReleaseTitle ?? history.SourceTitle,
                Quality = history.Quality,
                ReleaseGroup = parsed?.ReleaseGroup,
                ExtraInfo = new Dictionary<string, object>
                {
                    { "Size", size }
                }
            };

            return ParseCustomFormat(info);
        }
    }
}

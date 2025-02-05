﻿using Dapper;
using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.Housekeeping.Housekeepers
{
    public class CleanupOrphanedHistoryItems : IHousekeepingTask
    {
        private readonly IMainDatabase _database;

        public CleanupOrphanedHistoryItems(IMainDatabase database)
        {
            _database = database;
        }

        public void Clean()
        {
            CleanupOrphanedByArtist();
            CleanupOrphanedByAlbum();
        }

        private void CleanupOrphanedByArtist()
        {
            using (var mapper = _database.OpenConnection())
            {
                mapper.Execute(@"DELETE FROM ""History""
                                     WHERE ""Id"" IN (
                                     SELECT ""History"".""Id"" FROM ""History""
                                     LEFT OUTER JOIN ""Artists""
                                     ON ""History"".""ArtistId"" = ""Artists"".""Id""
                                     WHERE ""Artists"".""Id"" IS NULL)");
            }
        }

        private void CleanupOrphanedByAlbum()
        {
            using (var mapper = _database.OpenConnection())
            {
                mapper.Execute(@"DELETE FROM ""History""
                                     WHERE ""Id"" IN (
                                     SELECT ""History"".""Id"" FROM ""History""
                                     LEFT OUTER JOIN ""Albums""
                                     ON ""History"".""AlbumId"" = ""Albums"".""Id""
                                     WHERE ""Albums"".""Id"" IS NULL)");
            }
        }
    }
}

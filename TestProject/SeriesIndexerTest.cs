using NUnit.Framework;
using SeriesSelector;

namespace TestProject
{
    [TestFixture]
    public class SeriesIndexerTest
    {
        private SeriesIndexer testee;

        [SetUp]
        public void Setup()
        {
            this.testee = new SeriesIndexer();
        }

        [Test]
        public void IndexesCorrectForRawFile_Formatsxxexx()
        {
            var input = "the.walking.dead.s11e05.german.dl.1080p.web.h264.internal-wayne.mkv";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsTrue(result.HasIndexes);
            Assert.AreEqual(11, result.SeasonIndex);
            Assert.AreEqual(5, result.EpisodeIndex);
        }

        [Test]
        public void IndexesCorrectForRawFile2_FromatSxxExx()
        {
            var input = "The.Walking.Dead.S11E10.GERMAN.DL.DUBBED.1080p.WEB.h264-VoDTv.mkv";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsTrue(result.HasIndexes);
            Assert.AreEqual(11, result.SeasonIndex);
            Assert.AreEqual(10, result.EpisodeIndex);
        }

        [Test]
        public void IndexesCorrectForRawFile_SingleSeasonIndex()
        {
            var input = "Malcolm Mittendrin - 1x11 - Der Sündenbock.mpg";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsTrue(result.HasIndexes);
            Assert.AreEqual(1, result.SeasonIndex);
            Assert.AreEqual(11, result.EpisodeIndex);
        }

        [Test]
        public void IndexesCorrectForRawFile3_TripleNumberedIndexFromatxxx()
        {
            var input = "tvs-jerks-7p-mdhd-avc-107-aaaa.mkv";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsTrue(result.HasIndexes);
            Assert.AreEqual(1, result.SeasonIndex);
            Assert.AreEqual(7, result.EpisodeIndex);
        }

        [Test]
        public void IndexesCorrectForRawFile4_IndexAtEnd()
        {
            var input = "tvs-jerks-7p-mdhd-avc-S05E07.mkv";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsTrue(result.HasIndexes);
            Assert.AreEqual(5, result.SeasonIndex);
            Assert.AreEqual(7, result.EpisodeIndex);
        }

        [Test]
        public void IndexesCorrectForRawFile_TvRipNumberMadnessIndexAtEnd()
        {
            var input = "Morgen_hoer_ich_auf_S01E04_16.01.23_21-45_zdf_60_TVOON_DE.mpg.HD.avi";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsTrue(result.HasIndexes);
            Assert.AreEqual(1, result.SeasonIndex);
            Assert.AreEqual(4, result.EpisodeIndex);
        }

        [Test]
        public void IndexesCorrectForNamedFile()
        {
            var input = "Rick and Morty - 05x01 - Versauter Feind.mkv";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsTrue(result.HasIndexes);
            Assert.AreEqual(5, result.SeasonIndex);
            Assert.AreEqual(1, result.EpisodeIndex);
        }

        [Test]
        public void DetectsHighSeason()
        {
            var input = "Die Simpsons - 32x18 - Burger Kings.mkv";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsTrue(result.HasIndexes);
            Assert.AreEqual(32, result.SeasonIndex);
            Assert.AreEqual(18, result.EpisodeIndex);
        }

        [Test]
        public void IgnoresUpcomingNumbers()
        {
            var input = "Blacklist - 08x20 - Godwin Page (Nr. 141).mkv";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsTrue(result.HasIndexes);
            Assert.AreEqual(8, result.SeasonIndex);
            Assert.AreEqual(20, result.EpisodeIndex);
        }

        [Test]
        public void DetectsNameForNamedFile()
        {
            var input = "Rick and Morty - 05x01 - Versauter Feind.mkv";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsTrue(result.HasEpisodeName);
            Assert.AreEqual("Versauter Feind", result.EpisodeName);
        }



        [Test]
        public void DetectsNameForNonNamedFile()
        {
            var input = "The.Walking.Dead.S11E10.GERMAN.DL.DUBBED.1080p.WEB.h264-VoDTv.mkv";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsFalse(result.HasEpisodeName);
        }
        [Test]
        public void DetectsNameForNonNamedFile2()
        {
            var input = "Morgen_hoer_ich_auf_S01E04_16.01.23_21-45_zdf_60_TVOON_DE.mpg.HD.avi";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsFalse(result.HasEpisodeName);
        }

        [Test]
        public void DetectsNameForOtherFileEndingAndÜ()
        {
            var input = "Malcolm Mittendrin - 1x11 - Der Sündenbock.mpg";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsTrue(result.HasEpisodeName);
            Assert.AreEqual("Der Sündenbock", result.EpisodeName); // todo: casing
        }

        [Test]
        public void DetectsNameCasingCorrectly()
        {
            var input = "Rick and Morty - 05x01 - Versauter Feind(heute) (heute) und morgen-nicht-Nicht... immer.mkv";
            var result = this.testee.GetSeasonAndIndex(input);
            Assert.IsTrue(result.HasEpisodeName);
            Assert.AreEqual("Versauter Feind(heute) (heute) und morgen-nicht-Nicht... immer", result.EpisodeName);
        }

    }
}
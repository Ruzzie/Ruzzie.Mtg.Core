using System;
using System.IO;
using Moq;
using NUnit.Framework;
using Ruzzie.Mtg.Core.IO;

namespace Ruzzie.Mtg.Core.UnitTests.IO
{
    [TestFixture]
    public class RemoteRemoteFileLoaderTests
    {
        [Test]
        public void DontDownloadIfOlder()
        {
            //Arrange
            string filename = Guid.NewGuid() + ".json";
            File.Create(filename).Close(); //touch new file

            Mock<IFileDownloader> mockDownloader = new Mock<IFileDownloader>(MockBehavior.Strict);
            mockDownloader.Setup(downloader => downloader.MetaData)
                .Returns(new RemoteFileMetaData {LastModifiedTimeUtc = DateTime.UtcNow.Subtract(new TimeSpan(265, 0, 0))});

            IFileDownloader fileDownloader = mockDownloader.Object;

            //Act
            IRemoteFileLoader remoteFileLoader = new RemoteFileLoader(fileDownloader, filename, ".\\");

            remoteFileLoader.GetLocalOrDownloadIfNewer();

            //Assert
            mockDownloader.Verify();
        }

        [Test]
        public void DownloadIfNewer()
        {
            //Arrange
            string filename = Guid.NewGuid() + ".json";
            File.Create(filename).Close(); //touch new file

            Mock<IFileDownloader> mockDownloader = new Mock<IFileDownloader>(MockBehavior.Strict);
            mockDownloader.Setup(downloader => downloader.MetaData).Returns(new RemoteFileMetaData {LastModifiedTimeUtc = DateTime.UtcNow.AddYears(1)});
            mockDownloader.Setup(downloader => downloader.DownloadFile(".\\" + filename));

            IFileDownloader fileDownloader = mockDownloader.Object;

            //Act
            IRemoteFileLoader remoteFileLoader = new RemoteFileLoader(fileDownloader, filename, ".\\");

            remoteFileLoader.GetLocalOrDownloadIfNewer();

            //Assert
            mockDownloader.Verify();
        }

        [Test]
        public void DownloadIfNotExist()
        {
            //Arrange
            string filename = Guid.NewGuid() + ".json";
            //File.Create(filename).Close(); //touch new file

            Mock<IFileDownloader> mockDownloader = new Mock<IFileDownloader>(MockBehavior.Strict);
            mockDownloader.Setup(downloader => downloader.MetaData).Returns(new RemoteFileMetaData {LastModifiedTimeUtc = DateTime.UtcNow.AddYears(1)});
            mockDownloader.Setup(downloader => downloader.DownloadFile(".\\" + filename));

            IFileDownloader fileDownloader = mockDownloader.Object;

            //Act
            IRemoteFileLoader remoteFileLoader = new RemoteFileLoader(fileDownloader, filename, ".\\");

            remoteFileLoader.GetLocalOrDownloadIfNewer();

            //Assert
            mockDownloader.Verify();
        }
    }
}
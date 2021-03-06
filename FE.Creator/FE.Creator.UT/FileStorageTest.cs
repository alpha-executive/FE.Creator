﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FE.Creator.FileStorage;
using System.IO;
using System.Threading.Tasks;

namespace FE.Creator.UT
{
    /// <summary>
    /// Summary description for FileStorageTest
    /// </summary>
    [TestClass]
    public class FileStorageTest
    {
        public FileStorageTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestThumbnailGenerate()
        {
           var thumb  = WindowsThumbnailProvider.GetThumbnail(@"C:\Workspace\ux1hslyh.qku.jpg", 256, 256, ThumbnailOptions.None);

            thumb.Save(@"C:\Workspace\thumbinal.bmp");
        }

        [TestMethod]
        public void TestGeneralThumbinalGenerate()
        {
            var thumb = SimpleFileThumbinalGenerator.GetThumbnail(@"C:\Workspace\temp.docx", 256, 256);
            thumb.Save(@"C:\Workspace\thumbinal.bmp");
        }

        [TestMethod]
        public void TestImageExifConversion()
        {
            LocalFileSystemStorage fileStorage = new LocalFileSystemStorage(@"C:\Workspace\");
            byte[] contents = File.ReadAllBytes(@"C:\Workspace\personal\images\2017-05\IMG_3861.JPG");
            Task<FileStorageInfo> t = fileStorage.SaveFile(contents,
                ".jpg", true);

            Assert.IsNotNull(t.Result.FileFriendlyName);
        }
    }
}

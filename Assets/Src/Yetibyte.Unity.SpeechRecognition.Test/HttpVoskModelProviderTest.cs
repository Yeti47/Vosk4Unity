using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Yetibyte.Unity.SpeechRecognition.ModelManagement;

namespace Yetibyte.Unity.SpeechRecognition.Test
{
    public class HttpVoskModelProviderTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void FetchModels()
        {
            HttpVoskModelProvider provider = new HttpVoskModelProvider();

            bool success = provider.FetchModels();

            Assert.IsTrue(success);

        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator FetchModelsFull()
        {
            HttpVoskModelProvider provider = new HttpVoskModelProvider();

            bool success = provider.FetchModels();

            if(!success)
            {
                Assert.Fail("Error fetching models.");
                yield break;
            }

            DateTime startTime = DateTime.Now;

            while(provider.Status == VoskModelProviderStatus.Processing)
            {
                yield return null;

                if((DateTime.Now - startTime).TotalSeconds > 5)
                {
                    Assert.Fail("Timeout");
                    yield break;
                }
            }

            Assert.IsTrue(provider.Models.Any());
        }
    }
}

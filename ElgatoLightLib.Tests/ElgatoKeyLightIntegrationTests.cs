using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace eelightlib.Tests
{
    /// <exclude />
    [TestClass]
    public class ElgatoKeyLightIntegrationTests
    {
        private readonly ElgatoLightMgr elgatoKeyLightMgr = new ElgatoLightMgr();
        private IList<ElgatoLight> _lights;

        [TestInitialize]
        public void StartUp()
        {
            Task.Run(async () =>
            {
                _lights = await elgatoKeyLightMgr.StartDiscoverAsync();

                Assert.IsNotNull(_lights, "No Lights Found during the discovery.");

                if (_lights == null || _lights.Count == 0)
                {
                    Assert.Fail("No Light Found.");
                }

            }).GetAwaiter().GetResult();
        }

        internal ElgatoLight Light
        {
            get { return _lights[0]; }
        }

        /// <exclude />
        [TestMethod]
        public void DiscoveryTest()
        {
            Assert.IsTrue(Light.Port > 0, "Port wasn't parsed.");
            Assert.IsNotNull(Light.ToInfo());
        }

        /// <exclude />
        [TestMethod]
        public void OnTest()
        {
            Task.Run(async () =>
            {
                await Light.OnAsync();

                Assert.IsTrue(Light.On, $"Failed to turn on light.");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void OffTest()
        {
            Task.Run(async () =>
            {
                await Light.OffAsync();

                Assert.IsTrue(Light.On == false, $"Failed to turn off light.");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void IncreaseBrightnessTest()
        {
            Task.Run(async () =>
            {
                if (Light.On == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetBrightnessAsync(40);

                int initialBrightnessValue = Light.Brightness;

                int newBrightnessValue = initialBrightnessValue + 10;

                await Light.IncreaseBrightnessAsync(10);

                Assert.IsTrue(Light.Brightness == newBrightnessValue, $"Failed to increase the brightness of the light. The initial value was {initialBrightnessValue} the new value is {Light.Brightness}.");


            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void DecreaseBrightnessTest()
        {
            Task.Run(async () =>
            {
                if (Light.On == false)
                {
                    await Light.OnAsync();
                }

                int initialBrightnessValue = Light.Brightness;

                int newBrightnessValue = initialBrightnessValue - 10;

                await Light.DecreaseBrightnessAsync(10);

                Assert.IsTrue(Light.Brightness == newBrightnessValue, $"Failed to decrease the brightness of the light. The initial value was {initialBrightnessValue} the new value is {Light.Brightness}.");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void SetOneHundredPercentBrightnessAsync()
        {
            Task.Run(async () =>
            {
                if (Light.On == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetBrightnessAsync(100);

                Assert.IsTrue(Light.Brightness == 100);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void SetZeroBrightnessAsync()
        {
            Task.Run(async () =>
            {
                if (Light.On == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetBrightnessAsync(0);

                Assert.IsTrue(Light.Brightness == 0, $"Failed to set the brightness to 0%. Current brightness is set to {Light.Brightness}.");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void SetFiftyPercentBrightnessAsync()
        {
            Task.Run(async () =>
            {
                if (Light.On == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetBrightnessAsync(50);

                Assert.IsTrue(Light.Brightness == 50, $"Failed to set the brightness to 50%. Current brightness is set to {Light.Brightness}.");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void SetColorTemperatureTest()
        {
            Task.Run(async () =>
            {
                await Light.SetColorTemperatureAsync(4000);

                Assert.IsTrue(Light.Temperature == 4000);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void IncreaseColorTemperatureAsyncTest()
        {
            Task.Run(async () =>
            {
                if (Light.On == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetColorTemperatureAsync(2900);

                int initialTemperatureValue = Light.Temperature;

                int newTemperatureValue = initialTemperatureValue + 1000;

                await Light.IncreaseColorTemperatureAsync(1000);

                Assert.IsTrue(Light.Temperature == newTemperatureValue, $"Failed to increase the temperature of the light. The initial value was {initialTemperatureValue} the new value is {Light.Temperature}.");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void DecreaseColorTemperatureAsyncTest()
        {
            Task.Run(async () =>
            {
                if (Light.On == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetColorTemperatureAsync(2900);

                int initialColorTemperatureValue = Light.Temperature;

                int newColorTemperatureValue = initialColorTemperatureValue + 1000;

                await Light.DecreaseColorTemperatureAsync(1000);

                Assert.IsTrue(Light.Temperature == newColorTemperatureValue, $"Failed to decrease the temperature of the light. The initial value was {initialColorTemperatureValue} the new value is {Light.Temperature}.");


            }).GetAwaiter().GetResult();
        }
    }
}

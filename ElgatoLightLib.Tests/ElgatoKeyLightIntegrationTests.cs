using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ElgatoLightLib.Tests
{
    /// <exclude />
    [TestClass]
    public class ElgatoKeyLightIntegrationTests
    {
        private readonly ElgatoLightMgr elgatoKeyLightMgr = new();
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
        public void DiscoveryAndSettingsTest()
        {
            Assert.IsTrue(Light.Port > 0, "Port wasn't parsed.");

            Assert.IsNotNull(Light.ToString());

            Assert.IsNotNull(Light.Settings, "Failed to populate the Light Settings");
            Assert.IsTrue(Light.Settings.ColorChangeDurationMs > 0, "Failed to Read the ColorChangeDurationMs.");
            Assert.IsTrue(Light.Settings.PowerOnBehavior > 0, "Failed to Read the PowerOnBehavior.");
            Assert.IsTrue(Light.Settings.PowerOnBrightness > 0, "Failed to Read the PowerOnBrightness.");
            Assert.IsTrue(Light.Settings.PowerOnTemperature > 0, "Failed to Read the PowerOnTemperature.");
            Assert.IsTrue(Light.Settings.SwitchOffDurationMs > 0, "Failed to Read the SwitchOffDurationMs.");
            Assert.IsTrue(Light.Settings.SwitchOnDurationMs > 0, "Failed to Read the SwitchOnDurationMs.");

        }

        /// <exclude />
        [TestMethod]
        public void OnTest()
        {
            Task.Run(async () =>
            {
                await Light.OnAsync();

                Assert.IsTrue(Light.IsOn, $"Failed to turn on light.");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void OffTest()
        {
            Task.Run(async () =>
            {
                await Light.OffAsync();

                Assert.IsTrue(Light.IsOn == false, $"Failed to turn off light.");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void IncreaseBrightnessTest()
        {
            Task.Run(async () =>
            {
                if (Light.IsOn == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetBrightnessAsync(ElgatoLight.HalfBrightness);

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
                if (Light.IsOn == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetBrightnessAsync(ElgatoLight.HalfBrightness);

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
                if (Light.IsOn == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetBrightnessAsync(ElgatoLight.MaximumBrightness);

                Assert.IsTrue(Light.Brightness == ElgatoLight.MaximumBrightness);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(ElgatoLightOutOfRangeException))]
        public void SetZeroBrightnessAsync()
        {
            Task.Run(async () =>
            {
                if (Light.IsOn == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetBrightnessAsync(ElgatoLight.MinimumBrightness - 1);

                Assert.IsTrue(Light.Brightness == 0, $"Failed, Minimum brightness is {ElgatoLight.MinimumBrightness} Current brightness is set to {Light.Brightness}.");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void SetFiftyPercentBrightnessAsync()
        {
            Task.Run(async () =>
            {
                if (Light.IsOn == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetBrightnessAsync(ElgatoLight.HalfBrightness);

                Assert.IsTrue(Light.Brightness == ElgatoLight.HalfBrightness, $"Failed to set the brightness to {ElgatoLight.HalfBrightness}%. Current brightness is set to {Light.Brightness}.");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void SetColorTemperatureTest()
        {
            Task.Run(async () =>
            {
                if (Light.IsOn == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetColorTemperatureAsync(ElgatoLight.DefaultTemperature);

                Assert.IsTrue(Light.Temperature == ElgatoLight.DefaultTemperature);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void IncreaseColorTemperatureAsyncTest()
        {
            Task.Run(async () =>
            {
                if (Light.IsOn == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetColorTemperatureAsync(ElgatoLight.DefaultTemperature);

                int initialTemperatureValue = Light.Temperature;

                int newTemperatureValue = initialTemperatureValue + 100;

                await Light.IncreaseColorTemperatureAsync(100);

                Assert.IsTrue(Light.Temperature == newTemperatureValue, $"Failed to increase the temperature of the light. The initial value was {initialTemperatureValue} the new value is {Light.Temperature}.");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void DecreaseColorTemperatureAsyncTest()
        {
            Task.Run(async () =>
            {
                if (Light.IsOn == false)
                {
                    await Light.OnAsync();
                }

                await Light.SetColorTemperatureAsync(ElgatoLight.DefaultTemperature);

                int initialColorTemperatureValue = Light.Temperature;

                int newColorTemperatureValue = initialColorTemperatureValue - 1;

                await Light.DecreaseColorTemperatureAsync(1);

                Assert.IsTrue(Light.Temperature == newColorTemperatureValue, $"Failed to decrease the temperature of the light. The initial value was {initialColorTemperatureValue} the new value is {Light.Temperature}.");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void KeyLightTypeFoundTest()
        {
            Task.Run(async () =>
            {
                if (Light.IsOn == false)
                {
                    await Light.OnAsync();
                }

                Assert.IsTrue(Light.LightType == ElgatoLightType.KeyLight, "Failed to get the proper light type.");

            }).GetAwaiter().GetResult();
        }

    }
}

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using TechnicalTest.Server;

namespace TechnicalTest.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class AppServiceRegistrationTests
    {
        [TestMethod]
        public void Register_HappyScenario_RegistersAllServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterAppServices();

            // Assert
            services.Should().HaveCount(11);
        }
    }
}

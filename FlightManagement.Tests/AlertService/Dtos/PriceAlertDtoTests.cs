using FlightManagement.Common.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Tests.AlertService.Dtos
{
    public class PriceAlertDtoTests
    {
        private bool ValidateModel(object model, out List<ValidationResult> results)
        {
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, context, results, true);
        }

        [Fact]
        public void PriceAlertDto_ValidModel_ShouldPassValidation()
        {
            // Arrange
            var dto = new PriceAlertDto
            {
                UserId = Guid.NewGuid(),
                Origin = "NYC",
                Destination = "LAX",
                TargetPrice = 500,
                Currency = "USD",
                IsFlexibleDate = true,
                FlexibleDays = 5,
                OneWay = false,
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(10)
            };

            // Act
            var isValid = ValidateModel(dto, out var validationResults);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void PriceAlertDto_InvalidTargetPrice_ShouldFailValidation()
        {
            // Arrange (TargetPrice out of range)
            var dto = new PriceAlertDto
            {
                UserId = Guid.NewGuid(),
                Origin = "NYC",
                Destination = "LAX",
                TargetPrice = 100000, // Invalid (exceeds 10,000)
                Currency = "USD",
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(10)
            };

            // Act
            var isValid = ValidateModel(dto, out var validationResults);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Target price must be between 1 and 10,000"));
        }

        [Fact]
        public void PriceAlertDto_InvalidFlexibleDays_ShouldFailValidation()
        {
            // Arrange (FlexibleDays out of range)
            var dto = new PriceAlertDto
            {
                UserId = Guid.NewGuid(),
                Origin = "NYC",
                Destination = "LAX",
                TargetPrice = 500,
                Currency = "USD",
                IsFlexibleDate = true,
                FlexibleDays = 50, // Invalid (exceeds 30)
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(10)
            };

            // Act
            var isValid = ValidateModel(dto, out var validationResults);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Flexible days must be between 1 and 30"));
        }

        [Fact]
        public void PriceAlertDto_TooShortOrigin_ShouldFailValidation()
        {
            // Arrange
            var dto = new PriceAlertDto
            {
                UserId = Guid.NewGuid(),
                Origin = "NY", // Invalid (less than 3 characters)
                Destination = "LAX",
                TargetPrice = 500,
                Currency = "USD",
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(10)
            };

            // Act
            var isValid = ValidateModel(dto, out var validationResults);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Origin"));
        }
    }
}

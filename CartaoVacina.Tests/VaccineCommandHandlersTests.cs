using AutoMapper;
using CartaoVacina.Contracts.Data;
using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.DTOS.Vaccines;
using CartaoVacina.Core.Handlers.Commands.Vaccines;
using CartaoVacina.Core.Handlers.Queries;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CartaoVacina.Tests
{
    [TestClass]
    public class VaccineCommandHandlersTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IValidator<CreateVaccineDTO>> _createValidatorMock;
        private Mock<IValidator<UpdateVaccineDTO>> _updateValidatorMock;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _createValidatorMock = new Mock<IValidator<CreateVaccineDTO>>();
            _updateValidatorMock = new Mock<IValidator<UpdateVaccineDTO>>();
        }

        [TestMethod]
        public async Task CreateVaccineCommandHandler_ValidVaccine_ShouldCreate()
        {
            // Arrange
            var createDto = new CreateVaccineDTO { Name = "Test", Code = "TST", Doses = 2 };
            var vaccine = new Vaccine { Name = "Test", Code = "TST", Doses = 2 };
            var vaccineDto = new VaccineDTO { Id = 1, Name = "Test", Code = "TST", Doses = 2 };

            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _unitOfWorkMock.Setup(u => u.Vaccines.Get(It.IsAny<System.Func<Vaccine, bool>>()))
                .Returns(new List<Vaccine>());
            _mapperMock.Setup(m => m.Map<Vaccine>(createDto)).Returns(vaccine);
            _mapperMock.Setup(m => m.Map<VaccineDTO>(vaccine)).Returns(vaccineDto);
            _unitOfWorkMock.Setup(u => u.Vaccines.Add(vaccine, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var handler = new CreateVaccineCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _createValidatorMock.Object);
            var command = new CreateVaccineCommand(createDto);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(vaccineDto.Name, result.Name);
            Assert.AreEqual(vaccineDto.Code, result.Code);
        }

        [TestMethod]
        public async Task UpdateVaccineCommandHandler_ValidVaccine_ShouldUpdate()
        {
            // Arrange
            var updateDto = new UpdateVaccineDTO { Name = "Updated", Code = "UPD", Doses = 3 };
            var vaccine = new Vaccine { Id = 1, Name = "Test", Code = "TST", Doses = 2 };
            var updatedVaccine = new Vaccine { Id = 1, Name = "Updated", Code = "UPD", Doses = 3 };
            var vaccineDto = new VaccineDTO { Id = 1, Name = "Updated", Code = "UPD", Doses = 3 };

            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _unitOfWorkMock.Setup(u => u.Vaccines.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(vaccine);
            _unitOfWorkMock.Setup(u => u.Vaccines.Get(It.IsAny<System.Func<Vaccine, bool>>()))
                .Returns(new List<Vaccine>());
            _mapperMock.Setup(m => m.Map<VaccineDTO>(It.IsAny<Vaccine>())).Returns(vaccineDto);
            _unitOfWorkMock.Setup(u => u.Vaccines.Update(It.IsAny<Vaccine>()));
            _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var handler = new UpdateVaccineCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _updateValidatorMock.Object);
            var command = new UpdateVaccineCommand(1, updateDto);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(vaccineDto.Name, result.Name);
            Assert.AreEqual(vaccineDto.Code, result.Code);
        }

        [TestMethod]
        public async Task DeleteVaccineCommandHandler_ValidVaccine_ShouldDelete()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Vaccines.Delete(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var handler = new DeleteVaccineCommandHandler(_unitOfWorkMock.Object);
            var command = new DeleteVaccineCommand(1);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(u => u.Vaccines.Delete(1, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task ListVaccinesHandler_ValidVaccine_ShouldReturnList()
        {
            // Arrange
            var vaccines = new List<Vaccine> { new Vaccine { Id = 1, Name = "Test", Code = "TST", Doses = 2 } };
            var vaccineDtos = new List<VaccineDTO> { new VaccineDTO { Id = 1, Name = "Test", Code = "TST", Doses = 2 } };

            _unitOfWorkMock.Setup(u => u.Vaccines.Get()).Returns(vaccines.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<VaccineDTO>>(It.IsAny<IEnumerable<Vaccine>>())).Returns(vaccineDtos);

            var handler = new ListVaccinesHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var query = new ListVaccinesQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test", result[0].Name);
        }

        [TestMethod]
        public async Task CreateVaccineCommandHandler_InvalidVaccine_ShouldThrowValidationException()
        {
            // Arrange
            var createDto = new CreateVaccineDTO { Name = "", Code = "", Doses = 0 };
            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Name", "Name is required"),
                new FluentValidation.Results.ValidationFailure("Code", "Code is required")
            });
            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            var handler = new CreateVaccineCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _createValidatorMock.Object);
            var command = new CreateVaccineCommand(createDto);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<FluentValidation.ValidationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateVaccineCommandHandler_InvalidVaccine_ShouldThrowValidationException()
        {
            // Arrange
            var updateDto = new UpdateVaccineDTO { Name = "", Code = "", Doses = 0 };
            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Name", "Name is required"),
                new FluentValidation.Results.ValidationFailure("Code", "Code is required")
            });
            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            var handler = new UpdateVaccineCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _updateValidatorMock.Object);
            var command = new UpdateVaccineCommand(1, updateDto);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<FluentValidation.ValidationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateVaccineCommandHandler_InvalidId_ShouldThrowNotFoundException()
        {
            // Arrange
            var updateDto = new UpdateVaccineDTO { Name = "Test", Code = "TST", Doses = 1 };
            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _unitOfWorkMock.Setup(u => u.Vaccines.GetById(0, It.IsAny<CancellationToken>())).ReturnsAsync((Vaccine)null);
            var handler = new UpdateVaccineCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _updateValidatorMock.Object);
            var command = new UpdateVaccineCommand(0, updateDto);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<CartaoVacina.Core.Exceptions.NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task DeleteVaccineCommandHandler_InvalidId_ShouldThrowNotFoundException()
        {
            // Arrange
            var handler = new DeleteVaccineCommandHandler(_unitOfWorkMock.Object);
            var command = new DeleteVaccineCommand(0);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<CartaoVacina.Core.Exceptions.NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
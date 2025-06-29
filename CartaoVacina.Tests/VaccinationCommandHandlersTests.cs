using AutoMapper;
using CartaoVacina.Contracts.Data.DTOS.Vaccinations;
using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.Data.Interfaces;
using CartaoVacina.Core.Exceptions;
using CartaoVacina.Core.Handlers.Commands.Vaccinations;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CartaoVacina.Tests
{
    [TestClass]
    public class VaccinationCommandHandlersTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IValidator<CreateVaccinationDTO>> _createValidatorMock;
        private Mock<IValidator<UpdateVaccinationDTO>> _updateValidatorMock;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _createValidatorMock = new Mock<IValidator<CreateVaccinationDTO>>();
            _updateValidatorMock = new Mock<IValidator<UpdateVaccinationDTO>>();
        }

        [TestMethod]
        public async Task CreateVaccinationCommandHandler_ValidVaccination_ShouldCreate()
        {
            var createDto = new CreateVaccinationDTO { VaccineId = 1, VaccinationDate = DateTime.Now.AddDays(-1), Dose = 1 };
            var user = new User { Id = 1, Name = "Test User", BirthDate = DateTime.Now.AddYears(-20), Gender = Gender.Male };
            var vaccine = new Vaccine { Id = 1, Name = "Test Vaccine", Doses = 2 };
            var vaccination = new Vaccination { Id = 1, UserId = 1, VaccineId = 1, VaccinationDate = createDto.VaccinationDate, Dose = createDto.Dose, Vaccine = vaccine };
            var vaccinationDto = new VaccinationDTO { Id = 1, UserId = 1, VaccineId = 1, VaccinationDate = createDto.VaccinationDate, Dose = createDto.Dose, Vaccine = vaccine.Name };

            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _unitOfWorkMock.Setup(u => u.Users.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.Vaccines.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(vaccine);
            _unitOfWorkMock.Setup(u => u.Vaccinations.Get(It.IsAny<Func<Vaccination, bool>>())).Returns(new List<Vaccination>());
            _mapperMock.Setup(m => m.Map<VaccinationDTO>(It.IsAny<Vaccination>())).Returns(vaccinationDto);
            _unitOfWorkMock.Setup(u => u.Vaccinations.Add(It.IsAny<Vaccination>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var handler = new CreateVaccinationCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _createValidatorMock.Object);
            var command = new CreateVaccinationCommand(1, createDto);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(vaccinationDto.Vaccine, result.Vaccine);
            Assert.AreEqual(vaccinationDto.Dose, result.Dose);
        }

        [TestMethod]
        public async Task CreateVaccinationCommandHandler_InvalidVaccination_ShouldThrowException()
        {
            var createDto = new CreateVaccinationDTO { VaccineId = 0, VaccinationDate = DateTime.MinValue, Dose = 0 };
            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("VaccineId", "VaccineId is required"),
                new FluentValidation.Results.ValidationFailure("Dose", "Dose is required")
            });
            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            var handler = new CreateVaccinationCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _createValidatorMock.Object);
            var command = new CreateVaccinationCommand(1, createDto);

            await Assert.ThrowsExceptionAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateVaccinationCommandHandler_UserNotFound_ShouldThrowNotFoundException()
        {
            var createDto = new CreateVaccinationDTO { VaccineId = 1, VaccinationDate = DateTime.Now.AddDays(-1), Dose = 1 };
            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _unitOfWorkMock.Setup(u => u.Users.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
            var handler = new CreateVaccinationCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _createValidatorMock.Object);
            var command = new CreateVaccinationCommand(1, createDto);

            await Assert.ThrowsExceptionAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateVaccinationCommandHandler_VaccineNotFound_ShouldThrowNotFoundException()
        {
            var createDto = new CreateVaccinationDTO { VaccineId = 1, VaccinationDate = DateTime.Now.AddDays(-1), Dose = 1 };
            var user = new User { Id = 1, Name = "Test User", BirthDate = DateTime.Now.AddYears(-20), Gender = Gender.Male };
            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _unitOfWorkMock.Setup(u => u.Users.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.Vaccines.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync((Vaccine)null);
            var handler = new CreateVaccinationCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _createValidatorMock.Object);
            var command = new CreateVaccinationCommand(1, createDto);

            await Assert.ThrowsExceptionAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateVaccinationCommandHandler_ValidVaccination_ShouldUpdate()
        {
            var updateDto = new UpdateVaccinationDTO { VaccinationDate = DateTime.Now.AddDays(-1) };
            var user = new User { Id = 1, Name = "Test User", BirthDate = DateTime.Now.AddYears(-20), Gender = Gender.Male };
            var vaccination = new Vaccination { Id = 1, UserId = 1, VaccineId = 1, VaccinationDate = DateTime.Now.AddDays(-2), Dose = 1 };
            var vaccinationDto = new VaccinationDTO { Id = 1, UserId = 1, VaccineId = 1, VaccinationDate = updateDto.VaccinationDate.Value, Dose = 1, Vaccine = "Test Vaccine" };

            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _unitOfWorkMock.Setup(u => u.Users.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.Vaccinations.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(vaccination);
            _mapperMock.Setup(m => m.Map<VaccinationDTO>(It.IsAny<Vaccination>())).Returns(vaccinationDto);
            _unitOfWorkMock.Setup(u => u.Vaccinations.Update(It.IsAny<Vaccination>()));
            _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var handler = new UpdateVaccinationCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _updateValidatorMock.Object);
            var command = new UpdateVaccinationCommand(1, 1, updateDto);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(vaccinationDto.VaccinationDate, result.VaccinationDate);
        }

        [TestMethod]
        public async Task UpdateVaccinationCommandHandler_InvalidVaccination_ShouldThrowValidationException()
        {
            var updateDto = new UpdateVaccinationDTO { VaccinationDate = null };
            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("VaccinationDate", "VaccinationDate is required")
            });
            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            var handler = new UpdateVaccinationCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _updateValidatorMock.Object);
            var command = new UpdateVaccinationCommand(1, 1, updateDto);

            await Assert.ThrowsExceptionAsync<FluentValidation.ValidationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateVaccinationCommandHandler_UserNotFound_ShouldThrowNotFoundException()
        {
            var updateDto = new UpdateVaccinationDTO { VaccinationDate = DateTime.Now.AddDays(-1) };
            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _unitOfWorkMock.Setup(u => u.Users.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
            var handler = new UpdateVaccinationCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _updateValidatorMock.Object);
            var command = new UpdateVaccinationCommand(1, 1, updateDto);

            await Assert.ThrowsExceptionAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateVaccinationCommandHandler_VaccinationNotFound_ShouldThrowNotFoundException()
        {
            var updateDto = new UpdateVaccinationDTO { VaccinationDate = DateTime.Now.AddDays(-1) };
            var user = new User { Id = 1, Name = "Test User", BirthDate = DateTime.Now.AddYears(-20), Gender = Gender.Male };
            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _unitOfWorkMock.Setup(u => u.Users.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.Vaccinations.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync((Vaccination)null);
            var handler = new UpdateVaccinationCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _updateValidatorMock.Object);
            var command = new UpdateVaccinationCommand(1, 1, updateDto);

            await Assert.ThrowsExceptionAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task DeleteVaccinationCommandHandler_ValidVaccination_ShouldDelete()
        {
            var user = new User { Id = 1, Name = "Test User", BirthDate = DateTime.Now.AddYears(-20), Gender = Gender.Male };
            _unitOfWorkMock.Setup(u => u.Users.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.Vaccinations.Delete(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var handler = new DeleteVaccinationCommandHandler(_unitOfWorkMock.Object);
            var command = new DeleteVaccinationCommand(1, 1);

            await handler.Handle(command, CancellationToken.None);

            _unitOfWorkMock.Verify(u => u.Vaccinations.Delete(1, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteVaccinationCommandHandler_UserNotFound_ShouldThrowNotFoundException()
        {
            _unitOfWorkMock.Setup(u => u.Users.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
            var handler = new DeleteVaccinationCommandHandler(_unitOfWorkMock.Object);
            var command = new DeleteVaccinationCommand(1, 1);

            await Assert.ThrowsExceptionAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task DeleteVaccinationCommandHandler_VaccinationNotFound_ShouldThrowNotFoundException()
        {
            var user = new User { Id = 1, Name = "Test User", BirthDate = DateTime.Now.AddYears(-20), Gender = Gender.Male };
            _unitOfWorkMock.Setup(u => u.Users.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            var handler = new DeleteVaccinationCommandHandler(_unitOfWorkMock.Object);
            var command = new DeleteVaccinationCommand(1, 0);

            await Assert.ThrowsExceptionAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
} 
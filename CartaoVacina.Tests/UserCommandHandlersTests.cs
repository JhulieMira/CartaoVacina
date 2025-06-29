using AutoMapper;
using CartaoVacina.Contracts.Data.DTOS.Users;
using CartaoVacina.Contracts.Data.Entities;
using CartaoVacina.Contracts.Data.Interfaces;
using CartaoVacina.Core.Exceptions;
using CartaoVacina.Core.Handlers.Commands.Users;
using CartaoVacina.Core.Handlers.Queries.Users;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MockQueryable.Moq;

namespace CartaoVacina.Tests
{
    [TestClass]
    public class UserCommandHandlersTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IValidator<CreateUserDTO>> _createValidatorMock;
        private Mock<IValidator<UpdateUserDTO>> _updateValidatorMock;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _createValidatorMock = new Mock<IValidator<CreateUserDTO>>();
            _updateValidatorMock = new Mock<IValidator<UpdateUserDTO>>();
        }

        [TestMethod]
        public async Task CreateUserCommandHandler_ValidUser_ShouldCreate()
        {
            var createDto = new CreateUserDTO { Name = "Test User", BirthDate = DateTime.Now.AddYears(-20), Gender = "Male" };
            var user = new User { Id = 1, Name = "Test User", BirthDate = createDto.BirthDate, Gender = Gender.Male };
            var userDto = new UserDTO { Id = 1, Name = "Test User", BirthDate = createDto.BirthDate, Gender = "Male" };

            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mapperMock.Setup(m => m.Map<User>(createDto)).Returns(user);
            _mapperMock.Setup(m => m.Map<UserDTO>(user)).Returns(userDto);
            _unitOfWorkMock.Setup(u => u.Users.Add(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var handler = new CreateUserCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _createValidatorMock.Object);
            var command = new CreateUserCommand(createDto);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(userDto.Name, result.Name);
            Assert.AreEqual(userDto.BirthDate, result.BirthDate);
            Assert.AreEqual(userDto.Gender, result.Gender);
        }

        [TestMethod]
        public async Task CreateUserCommandHandler_InvalidUser_ShouldThrowValidationException()
        {
            var createDto = new CreateUserDTO { Name = "", BirthDate = DateTime.MinValue, Gender = "" };
            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Name", "Name is required"),
                new FluentValidation.Results.ValidationFailure("Gender", "Gender is required")
            });
            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            var handler = new CreateUserCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _createValidatorMock.Object);
            var command = new CreateUserCommand(createDto);

            await Assert.ThrowsExceptionAsync<FluentValidation.ValidationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateUserCommandHandler_ValidUser_ShouldUpdate()
        {
            var updateDto = new UpdateUserDTO { Name = "Updated User", BirthDate = DateTime.Now.AddYears(-25) };
            var user = new User { Id = 1, Name = "Test User", BirthDate = DateTime.Now.AddYears(-20), Gender = Gender.Male };
            var userDto = new UserDTO { Id = 1, Name = "Updated User", BirthDate = updateDto.BirthDate.Value, Gender = "Male" };

            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _unitOfWorkMock.Setup(u => u.Users.GetById(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserDTO>(user)).Returns(userDto);
            _unitOfWorkMock.Setup(u => u.Users.Update(It.IsAny<User>()));
            _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var handler = new UpdateUserCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _updateValidatorMock.Object);
            var command = new UpdateUserCommand(1, updateDto);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(userDto.Name, result.Name);
            Assert.AreEqual(userDto.BirthDate, result.BirthDate);
        }

        [TestMethod]
        public async Task UpdateUserCommandHandler_InvalidUser_ShouldThrowValidationException()
        {
            var updateDto = new UpdateUserDTO { Name = "", BirthDate = null };
            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Name", "Name is required")
            });
            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            var handler = new UpdateUserCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _updateValidatorMock.Object);
            var command = new UpdateUserCommand(1, updateDto);

            await Assert.ThrowsExceptionAsync<FluentValidation.ValidationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateUserCommandHandler_InvalidId_ShouldThrowNotFoundException()
        {
            var updateDto = new UpdateUserDTO { Name = "Test", BirthDate = DateTime.Now.AddYears(-30) };
            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _unitOfWorkMock.Setup(u => u.Users.GetById(0, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);
            var handler = new UpdateUserCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _updateValidatorMock.Object);
            var command = new UpdateUserCommand(0, updateDto);

            await Assert.ThrowsExceptionAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task DeleteUserCommandHandler_ValidUser_ShouldDelete()
        {
            _unitOfWorkMock.Setup(u => u.Users.Delete(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var handler = new DeleteUserCommandHandler(_unitOfWorkMock.Object);
            var command = new DeleteUserCommand(1);

            await handler.Handle(command, CancellationToken.None);

            _unitOfWorkMock.Verify(u => u.Users.Delete(1, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteUserCommandHandler_InvalidId_ShouldThrowNotFoundException()
        {
            var handler = new DeleteUserCommandHandler(_unitOfWorkMock.Object);
            var command = new DeleteUserCommand(0);

            await Assert.ThrowsExceptionAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }

        [TestMethod]
        public async Task ListUsersHandler_ShouldReturnList()
        {
            var users = new List<User> { new User { Id = 1, Name = "Test User", BirthDate = DateTime.Now.AddYears(-20), Gender = Gender.Male } };
            var userDtos = new List<UserDTO> { new UserDTO { Id = 1, Name = "Test User", BirthDate = DateTime.Now.AddYears(-20), Gender = "Male" } };

            _unitOfWorkMock.Setup(u => u.Users.Get()).Returns(users.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<UserDTO>>(It.IsAny<IEnumerable<User>>())).Returns(userDtos);

            var handler = new ListUsersHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var query = new ListUsersQuery();

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test User", result[0].Name);
        }


        [TestMethod]
        public async Task GetUserByIdHandler_InvalidId_ShouldThrowException()
        {
            var usersQueryable = new List<User>().AsQueryable();
            _unitOfWorkMock.Setup(u => u.Users.Get()).Returns(usersQueryable);
            var handler = new GetUserByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            var query = new GetUserByIdQuery(99);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() => handler.Handle(query, CancellationToken.None));
        }
    }
} 
using QLBS.Dtos.Author;
using QLBS.Models;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;

namespace QLBS.Services.Implementations
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<IEnumerable<AuthorResponseDto>> GetAllAuthorsAsync()
        {
            var authors = await _authorRepository.GetAllAsync();

            return authors.Select(a => new AuthorResponseDto
            {
                AuthorId = a.AuthorId,
                AuthorName = a.AuthorName,
                BirthYear = a.BirthYear,
                Nationality = a.Nationality
            });
        }

        public async Task<AuthorResponseDto?> GetAuthorByIdAsync(int id)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) return null;

            return new AuthorResponseDto
            {
                AuthorId = author.AuthorId,
                AuthorName = author.AuthorName,
                BirthYear = author.BirthYear,
                Nationality = author.Nationality
            };
        }

        public async Task<AuthorResponseDto?> CreateAuthorAsync(CreateAuthorDto createDto)
        {
            var author = new Author
            {
                AuthorName = createDto.AuthorName,
                BirthYear = createDto.BirthYear,
                Nationality = createDto.Nationality
            };

            try
            {
                var newAuthor = await _authorRepository.AddAsync(author);

                return new AuthorResponseDto
                {
                    AuthorId = newAuthor.AuthorId,
                    AuthorName = newAuthor.AuthorName,
                    BirthYear = newAuthor.BirthYear,
                    Nationality = newAuthor.Nationality
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateAuthorAsync(int id, UpdateAuthorDto updateDto)
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) return false;

            author.AuthorName = updateDto.AuthorName;
            author.BirthYear = updateDto.BirthYear;
            author.Nationality = updateDto.Nationality;

            return await _authorRepository.UpdateAsync(author);
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            return await _authorRepository.DeleteAsync(id);
        }
    }
}
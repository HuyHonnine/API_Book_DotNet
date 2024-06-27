﻿using Microsoft.EntityFrameworkCore;
using TestWebAPI.Data;
using TestWebAPI.Models;
using TestWebAPI.Repositories.Interfaces;

namespace TestWebAPI.Repositories
{
    public class CategoryRepositories : ICategoryRepositories
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepositories(ApplicationDbContext context) { 
            _context = context;
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<object> DeleteCategoryAsync(Category category)
        {
            _context.Categories!.Remove(category);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetCategoryAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Properties)
                .ThenInclude(p => p.PropertyHasDetail)
                .ToListAsync();
        }

        public async Task<Category> GetCategoryByTitleAsync(string title)
        {
            return await _context.Categories.FirstOrDefaultAsync(c=>c.title== title);
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.id == id);
        }
        public async Task<Category> GetFindAvatarAsync(string avatar)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.avatar == avatar);

        }

        public async Task<Category> UpdateCategoryAsync(Category oldCate, Category newCate)
        {
            oldCate.title = newCate.title;
            oldCate.description = newCate.description;
            oldCate.avatar = newCate.avatar;
            oldCate.updateAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return oldCate;
        }
    }
}

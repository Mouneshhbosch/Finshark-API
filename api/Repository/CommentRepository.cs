using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment is null)
            {
                return null;
            }
            _context.Remove(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments.Include(c => c.AppUser).ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            var comment = await _context.Comments.Include(x => x.AppUser).FirstOrDefaultAsync(c => c.Id == id);
            if (comment is null)
            {
                return null;
            }
            return comment;
        }

        public async Task<Comment?> UpdateAsync(int id, UpdateCommentDto updateCommentDto)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment is null)
            {
                return null;
            }
            comment.Title = updateCommentDto.Title;
            comment.Content = updateCommentDto.Content;
            _context.SaveChanges();
            return comment;
        }
    }
}
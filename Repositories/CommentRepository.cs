using api.Data;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class CommentRepository
(ApplicationDbContext context) : ICommentRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Comment> CreateAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);

        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task<Comment?> DeleteAsync(int id)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);

        if (comment == null)
        {
            return null;
        }

        _context.Comments.Remove(comment);

        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task<List<Comment>> GetAllAsync(CommentQueryObject queryObject)
    {
        var comments = _context.Comments.Include(x => x.AppUser).AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryObject.Symbol))
        {
            comments = comments.Where(x => x.Stock.Symbol.ToLower() == queryObject.Symbol.ToLower());
        }

        if (queryObject.IsDescending)
        {
            comments = comments.OrderBy(x => x.CreatedOn);
        }

        return await comments.ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(int id)
    => await _context.Comments.Include(x => x.AppUser).FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Comment?> UpdateAsync(int id, Comment comment)
    {
        var existingComment = await _context.Comments.FindAsync(id);

        if (existingComment == null)
        {
            return null;
        }

        existingComment.Title = comment.Title;
        existingComment.Content = comment.Content;

        await _context.SaveChangesAsync();

        return existingComment;
    }
}

using api.Data;
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

    public async Task<List<Comment>> GetAllAsync()
    => await _context.Comments.ToListAsync();

    public async Task<Comment?> GetByIdAsync(int id)
    => await _context.Comments.FindAsync(id);
}

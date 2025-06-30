using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;

    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Comment>> GetAllAsync()
    {
        return await _context.Comment.ToListAsync();
    }
    
    public async Task<Comment?> GetByIdAsync(int id)
    {
        return await _context.Comment.FindAsync(id);
    }
    
    public async Task<Comment> CreateAsync(Comment comment)
    {
        await _context.Comment.AddAsync(comment);
        await _context.SaveChangesAsync();
        return comment;
    }
}
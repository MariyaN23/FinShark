using api.Data;
using api.Dtos.Comment;
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

    public async Task<Comment?> UpdateAsync(int id, UpdateCommentRequestDto commentDto)
    {
        var comment = await _context.Comment.FindAsync(id);
        if (comment == null) return null;
        
        comment.Title = commentDto.Title;
        comment.Content = commentDto.Content;
        
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment?> DeleteAsync(int id)
    {
        var comment = await _context.Comment.FirstOrDefaultAsync(x => x.Id == id);
        if (comment == null) return null;
        _context.Comment.Remove(comment);
        await _context.SaveChangesAsync();
        return comment;
    }
}
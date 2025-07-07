using api.Data;
using api.Dtos.Comment;
using api.Helpers;
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

    public async Task<List<Comment>> GetAllAsync(CommentQueryObject queryObject)
    {
        //return await _context.Comments.Include(a => a.AppUser).ToListAsync();
        var comments = _context.Comments.Include(c => c.AppUser).AsQueryable();
        if (!string.IsNullOrWhiteSpace(queryObject.Symbol))
        {
            comments = comments.Where(s => s.Stock.Symbol == queryObject.Symbol);
        }

        if (queryObject.IsDescending)
        {
            comments = comments.OrderByDescending(s => s.Stock.Symbol);
        }
        return await comments.ToListAsync();
    }
    
    public async Task<Comment?> GetByIdAsync(int id)
    {
        return await _context.Comments.Include(a => a.AppUser).FirstOrDefaultAsync(c => c.Id == id);
    }
    
    public async Task<Comment> CreateAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment?> UpdateAsync(int id, UpdateCommentRequestDto commentDto)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null) return null;
        
        comment.Title = commentDto.Title;
        comment.Content = commentDto.Content;
        
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment?> DeleteAsync(int id)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
        if (comment == null) return null;
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return comment;
    }
}
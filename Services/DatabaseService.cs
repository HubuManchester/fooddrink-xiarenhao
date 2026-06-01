using SQLite;
using BiteRecord.Models;

namespace BiteRecord.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public DatabaseService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "biterecords.db3");
    }

    public void InitializeDatabase()
    {
        _database = new SQLiteAsyncConnection(_dbPath);
        _database.CreateTableAsync<BiteRecordModel>().Wait();
    }

    public async Task<List<BiteRecordModel>> GetAllRecordsAsync()
    {
        await EnsureDatabaseInitialized();
        return await _database.Table<BiteRecordModel>().OrderByDescending(r => r.CreatedDate).ToListAsync();
    }

    public async Task<BiteRecordModel?> GetRecordByIdAsync(int id)
    {
        await EnsureDatabaseInitialized();
        return await _database.Table<BiteRecordModel>().FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<int> SaveRecordAsync(BiteRecordModel record)
    {
        await EnsureDatabaseInitialized();
        if (record.Id == 0)
        {
            record.CreatedDate = DateTime.Now;
            return await _database.InsertAsync(record);
        }
        else
        {
            return await _database.UpdateAsync(record);
        }
    }

    public async Task<int> DeleteRecordAsync(BiteRecordModel record)
    {
        await EnsureDatabaseInitialized();
        return await _database.DeleteAsync(record);
    }

    public async Task<List<BiteRecordModel>> SearchRecordsAsync(string query)
    {
        await EnsureDatabaseInitialized();
        if (string.IsNullOrWhiteSpace(query))
            return await GetAllRecordsAsync();

        return await _database.Table<BiteRecordModel>()
            .Where(r => r.RestaurantName.ToLower().Contains(query.ToLower()) ||
                        r.DishName.ToLower().Contains(query.ToLower()) ||
                        r.ReviewText.ToLower().Contains(query.ToLower()))
            .OrderByDescending(r => r.CreatedDate)
            .ToListAsync();
    }

    private async Task EnsureDatabaseInitialized()
    {
        if (_database == null)
        {
            _database = new SQLiteAsyncConnection(_dbPath);
            await _database.CreateTableAsync<BiteRecordModel>();
        }
    }
}
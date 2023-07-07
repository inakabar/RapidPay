using Microsoft.EntityFrameworkCore;
using RapidPay.DataAccess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace RapidPay.DataAccess.Data
{
    public interface IDataAccessLayer
    {
        DbSet<Card> Cards { get; }
        DbSet<PaymentHistory> PaymentHistories { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

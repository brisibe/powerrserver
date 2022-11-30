using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using powerr.Models.Entities.MeterToken;

namespace powerr.repository.Configuration
{
    public class RechargeTokenConfiguration : IEntityTypeConfiguration<RechargeToken>
    {

        public void Configure(EntityTypeBuilder<RechargeToken> builder)
        {
            //Random random = new Random();

            //builder.HasData(

            //     new RechargeToken
            //     {
            //         RechargeTokenId = 1,
            //         Token = random.Next(10000000, 99999990),
            //         HasBeenUsed = false,
            //         Value = 500
            //     },

            //     new RechargeToken
            //     {
            //         RechargeTokenId = 2,
            //         Token = random.Next(10000000, 99999990),
            //         HasBeenUsed = false,
            //         Value = 500
            //     },

            //     new RechargeToken
            //     {
            //         RechargeTokenId = 3,
            //         Token = random.Next(10000000, 99999990),
            //         HasBeenUsed = false,
            //         Value = 500
            //     },

            //     new RechargeToken
            //     {
            //         RechargeTokenId = 4,
            //         Token = random.Next(10000000, 99999990),
            //         HasBeenUsed = false,
            //         Value = 500
            //     },

            //     new RechargeToken
            //     {
            //         RechargeTokenId = 5,
            //         Token = random.Next(10000000, 99999990),
            //         HasBeenUsed = false,
            //         Value = 20
            //     },

            //     new RechargeToken
            //     {
            //         RechargeTokenId = 6,
            //         Token = random.Next(10000000, 99999990),
            //         HasBeenUsed = false,
            //         Value = 20
            //     },

            //     new RechargeToken
            //     {
            //         RechargeTokenId = 7,
            //         Token = random.Next(10000000, 99999990),
            //         HasBeenUsed = false,
            //         Value = 20
            //     },

            //     new RechargeToken
            //     {
            //         RechargeTokenId = 8,
            //         Token = random.Next(10000000, 99999990),
            //         HasBeenUsed = false,
            //         Value = 20
            //     },

            //     new RechargeToken
            //     {
            //         RechargeTokenId = 9,
            //         Token = random.Next(10000000, 99999990),
            //         HasBeenUsed = false,
            //         Value = 20
            //     },
            //       new RechargeToken
            //       {
            //           RechargeTokenId = 10,
            //           Token = random.Next(10000000, 99999990),
            //           HasBeenUsed = false,
            //           Value = 20
            //       },
            //           new RechargeToken
            //           {
            //               RechargeTokenId = 11,
            //               Token = random.Next(10000000, 99999990),
            //               HasBeenUsed = false,
            //               Value = 40
            //           },

            //     new RechargeToken
            //     {
            //         RechargeTokenId = 12,
            //         Token = random.Next(10000000, 99999990),
            //         HasBeenUsed = false,
            //         Value = 40
            //     },

            //     new RechargeToken
            //     {
            //         RechargeTokenId = 7,
            //         Token = random.Next(10000000, 99999990),
            //         HasBeenUsed = false,
            //         Value = 40
            //     },

            //     new RechargeToken
            //     {
            //         RechargeTokenId = 8,
            //         Token = random.Next(10000000, 99999990),
            //         HasBeenUsed = false,
            //         Value = 20
            //     },

            //     new RechargeToken
            //     {
            //         RechargeTokenId = 9,
            //         Token = random.Next(10000000, 99999990),
            //         HasBeenUsed = false,
            //         Value = 20
            //     },
            //       new RechargeToken
            //       {
            //           RechargeTokenId = 10,
            //           Token = random.Next(10000000, 99999990),
            //           HasBeenUsed = false,
            //           Value = 20
            //       }
            //    ) ;
        }
    }
}

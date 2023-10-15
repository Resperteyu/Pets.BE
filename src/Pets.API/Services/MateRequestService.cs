using AutoMapper;
using Pets.API.Requests;
using Pets.Db;
using System.Threading.Tasks;
using System;
using Pets.Db.Models;
using Pets.API.Helpers;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Pets.API.Responses.Dtos;

namespace Pets.API.Services
{
    public interface IMateRequestService
    {
        Task<Guid> CreateMateRequest(CreateMateRequestRequest model);
        Task<List<MateRequestDto>> GetBy(Guid userId, MateRequestFilterType? type);
    }

    public class MateRequestService : IMateRequestService
    {
        private readonly PetsDbContext _context;
        private readonly IMapper _mapper;

        public MateRequestService(PetsDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Guid> CreateMateRequest(CreateMateRequestRequest model)
        {
            var mateRequest = _mapper.Map<MateRequest>(model);
            mateRequest.CreationDate = DateTime.UtcNow;
            mateRequest.MateRequestStateId = MateRequestStateConsts.SENT;

            var mateRequestRecord = await _context.MateRequests.AddAsync(mateRequest);
            await _context.SaveChangesAsync();

            return mateRequestRecord.Entity.Id;
        }

        public async Task<List<MateRequestDto>> GetBy(Guid userId, MateRequestFilterType? type)
        {
            IQueryable<MateRequest> mateRequests = _context.MateRequests
                                            .Include(i => i.PetProfile)
                                            .Include(i => i.PetMateProfile);
                                            
            if(type.HasValue)
            {
                switch(type.Value)
                {
                    case MateRequestFilterType.All:
                        mateRequests = mateRequests.Where(i => i.PetProfile.OwnerId == userId || i.PetMateProfile.OwnerId == userId);
                        break;
                    case MateRequestFilterType.Initiated:
                        mateRequests = mateRequests.Where(i => i.PetMateProfile.OwnerId == userId);
                        break;
                    case MateRequestFilterType.Received:
                        mateRequests = mateRequests.Where(i => i.PetProfile.OwnerId == userId);
                        break;
                }
            }
            else
            {
                mateRequests = mateRequests.Where(i => i.PetProfile.OwnerId == userId || i.PetMateProfile.OwnerId == userId);
            }

            mateRequests = mateRequests.OrderByDescending(x => x.CreationDate);

            return _mapper.Map<List<MateRequestDto>>(await mateRequests.ToListAsync());
        }
    }
}
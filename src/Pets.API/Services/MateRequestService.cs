﻿using AutoMapper;
using Pets.Db;
using System.Threading.Tasks;
using System;
using Pets.Db.Models;
using Pets.API.Helpers;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Pets.API.Responses.Dtos;
using Pets.API.Requests.MateRequest;

namespace Pets.API.Services
{
    public interface IMateRequestService
    {
        Task<Guid> CreateMateRequest(CreateMateRequestRequest model, Guid PetOwnerId, Guid PetMateOwnerId);
        Task<List<MateRequestDto>> Filter(Guid userId, MateRequestSearchParams mateRequestSearchParams);
        Task<MateRequestDto> GetById(Guid id, Guid ownerId);
        Task UpdateReply(PetMateRequestReplyRequest responseRequest);
        Task UpdateTransition(PetMateRequestTransitionRequest transitionRequest);
        Task UpdateDetails(PetMateRequestUpdateRequest updateRequest);
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

        public async Task<Guid> CreateMateRequest(CreateMateRequestRequest model, Guid petOwnerId, Guid petMateOwnerId)
        {
            var mateRequest = _mapper.Map<MateRequest>(model);
            mateRequest.CreationDate = DateTime.UtcNow;
            mateRequest.MateRequestStateId = MateRequestStateConsts.SENT;
            mateRequest.PetOwnerId = petOwnerId;
            mateRequest.PetMateOwnerId = petMateOwnerId;

            var mateRequestRecord = await _context.MateRequests.AddAsync(mateRequest);
            await _context.SaveChangesAsync();

            return mateRequestRecord.Entity.Id;
        }

        public async Task<List<MateRequestDto>> Filter(Guid userId, MateRequestSearchParams mateRequestSearchParams)
        {
            IQueryable<MateRequest> mateRequests = _context.MateRequests;
                                            
            if(mateRequestSearchParams.Type.HasValue)
            {
                switch(mateRequestSearchParams.Type.Value)
                {
                    case MateRequestType.All:
                        mateRequests = mateRequests.Where(i => i.PetOwnerId == userId || i.PetMateOwnerId == userId);
                        break;
                    case MateRequestType.Initiated:
                        mateRequests = mateRequests.Where(i => i.PetMateOwnerId == userId);
                        break;
                    case MateRequestType.Received:
                        mateRequests = mateRequests.Where(i => i.PetOwnerId == userId);
                        break;
                }
            }
            else
            {
                mateRequests = mateRequests.Where(i => i.PetOwnerId == userId || i.PetMateOwnerId == userId);
            }

            mateRequests = mateRequests.Include(i => i.MateRequestState)
                                        .Include(i => i.PetProfile)
                                        .Include(i => i.PetMateProfile)
                                        .OrderByDescending(x => x.CreationDate);

            return _mapper.Map<List<MateRequestDto>>(await mateRequests.ToListAsync());
        }

        public async Task<MateRequestDto> GetById(Guid id, Guid ownerId)
        {
            var mateRequest = await _context.MateRequests.Where(i => i.Id == id)
                                            .Include(i => i.MateRequestState)
                                            .Include(i => i.PetProfile.Owner)
                                            .Include(i => i.PetMateProfile.Owner)
                                            .SingleOrDefaultAsync();


            if (mateRequest == null)
                return null;

            var mateRequestDto = _mapper.Map<MateRequestDto>(mateRequest);
            
            mateRequestDto.IsRequester = mateRequestDto.PetMateOwnerId == ownerId;
            mateRequestDto.IsReceiver = mateRequestDto.PetOwnerId == ownerId;

            return mateRequestDto;
        }

        public async Task UpdateReply(PetMateRequestReplyRequest responseRequest)
        {
            var entity = await _context.MateRequests.Where(i => i.Id == responseRequest.MateRequestId)
                                      .Include(i => i.PetProfile)
                                      .Include(i => i.PetMateProfile)
                                      .SingleAsync();

            entity.MateRequestStateId = responseRequest.MateRequestStateId;
            entity.Response = responseRequest.Response;

            _context.MateRequests.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransition(PetMateRequestTransitionRequest transitionRequest)
        {
            var entity = await _context.MateRequests.Where(i => i.Id == transitionRequest.MateRequestId)
                                      .SingleAsync();

            entity.MateRequestStateId = transitionRequest.MateRequestStateId;
            entity.Comment = transitionRequest.Comment;

            _context.MateRequests.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDetails(PetMateRequestUpdateRequest updateRequest)
        {
            var entity = await _context.MateRequests.Where(i => i.Id == updateRequest.MateRequestId)
                                      .SingleAsync();

            entity.MateRequestStateId = MateRequestStateConsts.SENT;
            entity.Description = updateRequest.Description;
            entity.LitterSplitAgreement = updateRequest.LitterSplitAgreement;
            entity.BreedingPlaceAgreement = updateRequest.BreedingPlaceAgreement;
            entity.AmountAgreement = updateRequest.AmountAgreement;

            _context.MateRequests.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
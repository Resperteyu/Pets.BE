using Azure.Core;
using Pets.API.Responses.Dtos;

namespace Pets.API.Helpers
{
    public class MateRequestStateChangeValidatorResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
    }


    public interface IMateRequestStateChangeValidator
    {

        MateRequestStateChangeValidatorResult ValidateResponse(MateRequestDto mateRequest, byte responseState);
        MateRequestStateChangeValidatorResult ValidateTransition(MateRequestDto mateRequest, byte transitionState);
    }

    public class MateRequestStateChangeValidator : IMateRequestStateChangeValidator
    {
        public MateRequestStateChangeValidatorResult ValidateResponse(MateRequestDto mateRequest, byte responseState)
        {
            var result = new MateRequestStateChangeValidatorResult
            {
                Result = true
            };

            if (mateRequest.MateRequestState.Id != MateRequestStateConsts.SENT)
            {
                result.Result = false;
                result.Message = "Current state of mate-request doesn't allow this operation " + mateRequest.MateRequestState.Title;
                return result;
            }

            if (responseState != MateRequestStateConsts.ACCEPTED
                && responseState != MateRequestStateConsts.CHANGES_REQUESTED
                && responseState != MateRequestStateConsts.REJECTED)
            {
                result.Result = false;
                result.Message = "Invalid response state " + mateRequest.MateRequestState.Title;
                return result;
            }

            return result;
        }

        public MateRequestStateChangeValidatorResult ValidateTransition(MateRequestDto mateRequest, byte transitionState)
        {
            var result = new MateRequestStateChangeValidatorResult
            {
                Result = true
            };

            if (transitionState != MateRequestStateConsts.COMPLETED
                && transitionState != MateRequestStateConsts.FAILED
                && transitionState != MateRequestStateConsts.BREEDING)
            {
                result.Result = false;
                result.Message = "Invalid transition state";
                return result;
            }

            if (mateRequest.MateRequestState.Id == MateRequestStateConsts.ACCEPTED
            &&
            (transitionState != MateRequestStateConsts.FAILED && transitionState != MateRequestStateConsts.BREEDING)
                )
            {
                result.Result = false;
                result.Message = "Current state of mate-request doesn't allow this operation " + mateRequest.MateRequestState.Title;
                return result;
            }

            if (mateRequest.MateRequestState.Id == MateRequestStateConsts.BREEDING
            &&
            (transitionState != MateRequestStateConsts.FAILED && transitionState != MateRequestStateConsts.COMPLETED)
                )
            {
                result.Result = false;
                result.Message = "Current state of mate-request doesn't allow this operation " + mateRequest.MateRequestState.Title;
                return result;
            }

            if (mateRequest.MateRequestState.Id == MateRequestStateConsts.SENT
                && transitionState != MateRequestStateConsts.FAILED
                && !mateRequest.IsRequester)
            {
                result.Result = false;
                result.Message = "Current state of mate-request doesn't allow this operation " + mateRequest.MateRequestState.Title;
                return result;
            }

            return result;
        }
    }
}

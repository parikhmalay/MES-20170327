

namespace MES.DTO.Library.Base
{
    public enum UpdateOptions
    { /// <summary>
        /// Option indicates that only update data for IDs that are 0s
        /// </summary>
        CreateOnly = 1,

        /// <summary>
        /// Option indicates that create data for IDs that are 0 and update data for IDs that are not 0.
        /// </summary>
        CreateOrUpdate = 2,

        /// <summary>
        /// Option indicates that only update data for IDs that are greater than 0.
        /// </summary>
        UpdateOnly = 3,

        /// <summary>
        /// Used internally at BO Layer
        /// </summary>
        CreateInternal = 4,
        /// <summary>
        /// Used internally at BO Layer
        /// </summary>
        UpdateInternal = 5
    }
}

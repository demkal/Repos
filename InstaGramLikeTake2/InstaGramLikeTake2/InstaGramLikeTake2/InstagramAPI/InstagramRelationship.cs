using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace InstaGramLikeTake2
{
    public class InstagramRelationshipList
    {
        /// <summary>
        /// Creates an object of the container class from the given string in JSON format.
        /// </summary>
        /// <param name="jsonResponse">JSON formatted string.</param>
        /// <returns>Container object.</returns>
        public static InstagramRelationshipList CreateFromJsonResponse(String jsonResponse)
        {
            return JsonConvert.DeserializeObject<InstagramRelationshipList>(jsonResponse);
        }

        public List<UserData> Data { get; set; }
    }
}

﻿namespace Core.Utilities.Results.Concrete
{
    public class SuccesResult: Result
    {
        public SuccesResult(string message):base(true,message)
        {
            
        }
        public SuccesResult():base(true)
        {
                
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Comtek.DataOperationResultCore
{
    /// <summary>
    ///     A generic object to return after performing a data operation
    /// </summary>
    public class DataOperationResult
    {
        public bool Success { get; set; }
        public List<string> Messages { get; set; }

        public string Message
        {
            get
            {
                return Messages.Aggregate("", (current, message) => current + message);
            }
        }

        public DataOperationResult()
        {
            Success = true;
            Messages = new List<string>();
        }
        public DataOperationResult(Exception exception) : this()
        {
            Success = false;
            AddMessage(exception);
        }

        public void AddMessages(List<string> messages)
        {
            Messages.AddRange(messages);
        }

        public void AddMessage(string message)
        {
            Messages.Add(message);
        }

        public void AddMessage(Exception e)
        {
            AddExceptionMessage(e);
        }

        public void Failure()
        {
            Success = false;
        }

        public void Failure(string message)
        {
            Success = false;
            AddMessage(message);
        }

        public void Failure(List<string> messages)
        {
            Success = false;
            AddMessages(messages);
        }

        public void Failure(Exception e)
        {
            Success = false;
            AddMessage(e);
        }

        /// <summary>
        ///     Add another DataOperationResult to this one
        /// </summary>
        /// <param name="otherResult">The additional result to merge</param>
        public void MergeDataOperationResult(DataOperationResult otherResult)
        {
            Messages.AddRange(otherResult.Messages);
            // If the current success is false, leave it like that
            // otherwise update it from the other result
            if (Success) Success = otherResult.Success;
        }

        /// <summary>
        ///     Add the inner message text from an Exception
        /// </summary>
        /// <param name="e">Exception</param>
        public void AddExceptionMessage(Exception e)
        {
            var msg = GetExceptionMessage(e);

            Messages.Add(msg);
        }

        private static string GetExceptionMessage(Exception ex)
        {
            while (true)
            {
                if (ex.InnerException != null)
                {
                    return GetExceptionMessage(ex.InnerException);
                }

                var msg = ex.Message;

                return msg;
            }
        }

        /// <summary>
        ///     Check if a DateTime is set to min or max value
        /// </summary>
        /// <param name="date">The date to validate</param>
        /// <param name="message"></param>
        /// <param name="variableName"></param>
        /// <returns>True for a valid date, otherwise false</returns>
        public bool ValidateDate(DateTime date, string variableName, out string message)
        {
            message = "Date is valid";

            if (date != DateTime.MaxValue && date != DateTime.MinValue) return true;

            message = $"{date} is not a valid value for {variableName}";
            return false;
        }

        public bool ValidateDate(DateTime? date, string variableName, out string message)
        {
            message = "Date is valid";

            if (date != DateTime.MaxValue && date != DateTime.MinValue) return true;

            message = $"{date} is not a valid value for {variableName}";
            return false;
        }

        public bool ValidateString(string text, string variableName, out string message)
        {
            message = "String is not null";

            if (!string.IsNullOrEmpty(text)) return true;

            message = $"There was no text found for the field '{variableName}'";
            return false;
        }
    }

    /// <summary>
    ///     A generic object to return after performing a data operation, including the model that was involved
    /// </summary>
    public class DataOperationResult<T> : DataOperationResult
    {
        public T Model { get; set; }

        public DataOperationResult()
        {

        }

        public DataOperationResult(T obj)
        {
            Model = obj;
        }

        public void MergeDataOperationResult(DataOperationResult<T> otherResult)
        {
            Messages.AddRange(otherResult.Messages);
            // If the current success is false, leave it like that
            // otherwise update it from the other result
            if (Success) Success = otherResult.Success;
            Model = otherResult.Model;
        }
    }

}
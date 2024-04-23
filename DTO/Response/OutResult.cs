using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaricApp.DTO.Response
{
    public class OutResult
    {
        public bool result = false;
        public List<ValidationResult> Errors = new List<ValidationResult>();
    }

    /// <summary>
    /// Ответ полученный от сервера
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OutResult<T>
    {
        public IImmutableList<ValidationResult> Errors { get; set; }
        public T Data { get; set; }
        public OutResult(T data, IImmutableList<ValidationResult> errors)
        {
            Data = data;
            Errors = errors;
        }
    }
}

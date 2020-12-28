using System.Threading.Tasks;

namespace Hemera.Interfaces
{
    public interface IValidate
    {
        public Task<bool> ValidateInput();
    }
}

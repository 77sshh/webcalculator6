using Microsoft.AspNetCore.Mvc;
using webcalculator6.Data;
using Microsoft.AspNetCore.Components.Forms;
using webcalculator6.Models;
namespace webcalculator6.Controllers
{
    public enum Operation
    {
        Add, Subtract, Multiply, Divide
    }
    public class CalculatorController : Controller
    {
        private CalculatorContext _context;
        public CalculatorController(CalculatorContext context)
        {
            _context = context;
        }

         [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Calculate(double num1, double
       num2, Operation operation)
        {
            double result = 0;
            switch (operation)
            {
                case Operation.Add:
                    result = num1 + num2;
                    break;
                case Operation.Subtract:
                    result = num1 - num2;
                    break;
                case Operation.Multiply:
                    result = num1 * num2;
                    break;
                case Operation.Divide:
                    result = num1 / num2;
                    break;
            }
            ViewBag.Result = result;
            DataInputVariant dataInputVariant = new DataInputVariant();
            dataInputVariant.Operand_1 = num1.ToString();
            dataInputVariant.Operand_2 = num2.ToString();
            dataInputVariant.Type_operation = operation.
           ToString();
            dataInputVariant.Result = result.ToString();

            _context.DataInputVariants.Add(dataInputVariant);
            _context.SaveChanges();
            return View("Index");
        }
    }
}
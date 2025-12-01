using Microsoft.AspNetCore.Mvc;
using webcalculator6.Data;
using Microsoft.AspNetCore.Components.Forms;
using webcalculator6.Models;
using System.Text.Json;
using Confluent.Kafka;
using webcalculator6.Services;

namespace webcalculator6.Controllers
{
    public enum Operation
    {
        Add, Subtract, Multiply, Divide
    }
    public class CalculatorController : Controller
    {
        private readonly CalculatorContext _context;
        private readonly KafkaProducerService<Null, string> _producer;

        public CalculatorController(CalculatorContext context, KafkaProducerService<Null, string> producer)
        {
            _context = context;
            _producer = producer;
        }

         [HttpGet]
        public IActionResult Index()
        {
           var data = _context.DataInputVariants.OrderByDescending(x => x.ID_DataInputVariant).ToList();
            return View(data);
        }
        /// <summary>
        /// Обработка запроса на вычисление.
        /// </summary>
        /// <param name="num1">Первый операнд.</param>
        /// <param name="num2">Второй операнд.</param>
        /// <param name="operation">Тип операции (сложение, вычитание, умножение, деление).</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calculate(double num1, double num2, Operation operation)
        {
            // Подготовка объекта для расчета
            var dataInputVariant = new DataInputVariant
            {
                Operand_1 = num1,
                Operand_2 = num2,
                Type_operation = (Models.Operation)operation,
            };
            // Отправка данных в Kafka
            await SendDataToKafka(dataInputVariant);
            // Перенаправление на страницу Index
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Callback([FromBody] DataInputVariant inputData)
        {
            SaveDataAndResult(inputData);
            return Ok();
        }
        private DataInputVariant SaveDataAndResult(DataInputVariant inputData)
        {
            _context.DataInputVariants.Add(inputData);
            _context.SaveChanges();
            return inputData;
        }
       private async Task SendDataToKafka(DataInputVariant dataInputVariant)
        {
            var json = JsonSerializer.Serialize(dataInputVariant);
            await _producer.ProduceAsync("kolupaev", new Message<Null, string> { Value = json });
        }
    }
}
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebAppService.Interface;
using WebAppService.Service;
namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentservice _std;

        public StudentController(IStudentservice stdser)
        {
            _std = stdser;
        }

        [HttpGet]
        [Route("getAll")]
        public async Task<List<Student>> GetNames()
        {
            return await _std.getnames();
        }

        [HttpPost]
        [Route("addNewStd")]
        public async Task<bool> AddNewStd(Student student)
        {
            return await _std.addNewStd(student);
        }

        [HttpPut]
        [Route("updateStd")]
        public async Task<bool> UpdateStd(int id, Student student)
        {
            return await _std.updateStd(id, student);
        }

        [HttpDelete]
        [Route("deleteStd")]
        public async Task<bool> DeleteStd(int id)
        {
            return await _std.deleteStd(id);
        }

        [HttpPost("codeanalysis")]
        public ActionResult AnalyzeCode([FromBody] JsonElement codeAnalysisRequest)
        {
            // Check if the "code" field exists and is a string
            if (!codeAnalysisRequest.TryGetProperty("code", out JsonElement codeElement) || codeElement.ValueKind != JsonValueKind.String)
            {
                return BadRequest("Code input is null or empty.");
            }

            // Extract the code from the JSON element
            string code = codeElement.GetString();

            // Perform code analysis and calculate score
            int length = code.Length;
            int numberOfMethods = Regex.Matches(code, @"\b(public|private|protected|internal)\s+\w+\s+\w+\s*\(").Count;
            int numberOfComments = Regex.Matches(code, @"(\/\/.*?$|\/\*.*?\*\/)", RegexOptions.Singleline | RegexOptions.Multiline).Count;
            int complexity = CalculateComplexity(code);
            bool followsNamingConventions = CheckNamingConventions(code);
            bool hasErrorHandling = CheckErrorHandling(code);
            bool hasCodeDuplication = CheckCodeDuplication(code);
            bool hasGoodComments = CheckCommentQuality(code);
            bool hasConsistentFormatting = CheckCodeFormatting(code);

            // Calculate score
            int score = CalculateScore(length, numberOfMethods, numberOfComments, complexity, followsNamingConventions, hasErrorHandling, hasCodeDuplication, hasGoodComments, hasConsistentFormatting);

            // Prepare analysis result
            var analysisResult = new
            {
                Message = "Code analysis successful.",
                Length = length,
                NumberOfMethods = numberOfMethods,
                NumberOfComments = numberOfComments,
                Complexity = complexity,
                FollowsNamingConventions = followsNamingConventions,
                HasErrorHandling = hasErrorHandling,
                HasCodeDuplication = hasCodeDuplication,
                HasGoodComments = hasGoodComments,
                HasConsistentFormatting = hasConsistentFormatting,
                Score = score
            };

            return Ok(analysisResult);
        } 

        private int CalculateScore(int length, int numberOfMethods, int numberOfComments, int complexity, bool followsNamingConventions, bool hasErrorHandling, bool hasCodeDuplication, bool hasGoodComments, bool hasConsistentFormatting)
        {
            int score = 100;

            if (length > 1000)
            {
                score -= 10;
            }
            if (numberOfMethods > 10)
            {
                score -= 10;
            }
            if (numberOfComments < 5)
            {
                score -= 10;
            }
            if (complexity > 10)
            {
                score -= 20;
            }
            if (!followsNamingConventions)
            {
                score -= 15;
            }
            if (!hasErrorHandling)
            {
                score -= 10;
            }
            if (!hasCodeDuplication)
            {
                score -= 10;
            }
            if (!hasGoodComments)
            {
                score -= 10;
            }
            if (!hasConsistentFormatting)
            {
                score -= 5;
            }

            return score > 0 ? score : 0; // Ensure score is not negative
        }

        private int CalculateComplexity(string code)
        {
            // A simple complexity calculation based on number of conditionals and loops
            int complexity = 1; // Base complexity for the method
            complexity += Regex.Matches(code, @"\b(if|else if|switch|for|while|foreach|case)\b").Count;
            return complexity;
        }

        private bool CheckNamingConventions(string code)
        {
            Regex variableRegex = new Regex(@"\b([a-z]+[a-zA-Z0-9]*)\b");
            Regex classRegex = new Regex(@"\b([A-Z]+[a-zA-Z0-9]*)\b");

            // Find all matches for variables and classes
            MatchCollection variableMatches = variableRegex.Matches(code);
            MatchCollection classMatches = classRegex.Matches(code);

            // Check if any variable doesn't follow camelCase or any class doesn't follow PascalCase
            foreach (Match variableMatch in variableMatches)
            {
                if (!IsCamelCase(variableMatch.Value))
                {
                    return false;
                }
            }

            foreach (Match classMatch in classMatches)
            {
                if (!IsPascalCase(classMatch.Value))
                {
                    return false;
                }
            }

            return true; // All identifiers follow naming conventions
        }
        private bool IsCamelCase(string identifier)
        {
            // Check if the identifier follows camelCase convention
            // (first letter lowercase, subsequent words start with uppercase)
            return char.IsLower(identifier[0]) && !identifier.Contains("_");
        }

        private bool IsPascalCase(string identifier)
        {
            // Check if the identifier follows PascalCase convention
            // (first letter uppercase, subsequent words start with uppercase)
            return char.IsUpper(identifier[0]) && !identifier.Contains("_");
        }
        private bool CheckErrorHandling(string code)
        {
            // Check if there are file I/O operations without error handling
            if (code.Contains("File.Open") && !code.Contains("try") && !code.Contains("catch"))
            {
                return false; // File I/O operation without try-catch block
            }

            // Add more checks for other error-prone operations as needed

            return true; // All potentially error-prone operations are properly handled
        }

        private bool CheckCodeDuplication(string code)
        {
            var lines = code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(line => line.Trim())
                             .Where(line => !string.IsNullOrWhiteSpace(line))
                             .ToList();

            var lineHashes = new Dictionary<int, int>();

            for (int i = 0; i < lines.Count - 1; i++)
            {
                string linePair = lines[i] + lines[i + 1];
                int hash = linePair.GetHashCode();

                if (lineHashes.ContainsKey(hash))
                {
                    lineHashes[hash]++;
                    if (lineHashes[hash] > 1)
                    {
                        return false; // Duplicate code found
                    }
                }
                else
                {
                    lineHashes[hash] = 1;
                }
            }

            return true; // No duplicate code found
        }

        private bool CheckCommentQuality(string code)
        {// Check if there are comments present in the code
         // For simplicity, let's assume any comment is considered good quality
            return code.Contains("//") || code.Contains("/*");
        }

        private bool CheckCodeFormatting(string code)
        {
            // Check for consistent spacing around operators and keywords
            var operators = new[] { "+", "-", "*", "/", "=", "==", "!=", "<", ">", "<=", ">=", "&&", "||" };
            foreach (var op in operators)
            {
                var pattern = $@"\S{op}\S";
                if (Regex.IsMatch(code, pattern))
                {
                    return false; // No space around operator
                }
            }

            // Check for spaces after keywords (e.g., if, for, while)
            var keywords = new[] { "if", "for", "while", "switch", "catch" };
            foreach (var keyword in keywords)
            {
                var pattern = $@"\b{keyword}\(";
                if (Regex.IsMatch(code, pattern))
                {
                    return false; // No space after keyword
                }
            }

            // Check for consistent indentation
            var lines = code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(line => line.Trim())
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                            .ToList();

            for (int i = 1; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("}") && lines[i - 1].StartsWith("    "))
                {
                    return false; // Inconsistent indentation (expecting 4 spaces)
                }
            }

            return true; // All formatting checks passed
        }

    }
}

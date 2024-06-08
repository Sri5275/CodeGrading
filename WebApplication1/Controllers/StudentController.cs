using Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebAppService.Interface;
using WebAppService.Service;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;


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
            // Parse the code into a syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();

            // Extract method bodies
            var methodBodies = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                                   .Select(method => method.Body)
                                   .Where(body => body != null)
                                   .ToList();

            if (methodBodies == null || !methodBodies.Any())
            {
                return false; // No method bodies found, no duplication possible
            }

            // Tokenize method bodies
            var methodTokens = methodBodies
                .Select(body => string.Join(" ", body.DescendantTokens().Select(token => token.Text)))
                .ToList();

            // Compare method bodies for similarity
            var duplicatedMethods = new HashSet<string>();
            for (int i = 0; i < methodTokens.Count; i++)
            {
                for (int j = i + 1; j < methodTokens.Count; j++)
                {
                    if (AreMethodsSimilar(methodTokens[i], methodTokens[j]))
                    {
                        duplicatedMethods.Add($"Method {i + 1} and Method {j + 1}");
                    }
                }
            }

            if (duplicatedMethods.Any())
            {
                // For debugging purposes, print duplicated methods
                Console.WriteLine("Duplicated methods:");
                foreach (var methodPair in duplicatedMethods)
                {
                    Console.WriteLine(methodPair);
                }
                return true; // Duplicated methods found
            }

            return false; // No duplicated methods found
        }

        // Define the method to compare two methods' tokens for similarity
        private bool AreMethodsSimilar(string method1, string method2)
        {
            // Simple similarity check: if the methods' tokens are exactly the same, consider them similar
            // You can replace this with a more sophisticated similarity check if needed
            return method1 == method2;
        }
        private bool CheckCommentQuality(string code)
        {// Check if there are comments present in the code
         // For simplicity, let's assume any comment is considered good quality
            return code.Contains("//") || code.Contains("/*");
        }

        private bool CheckCodeFormatting(string code)
        {
            // Check for consistent indentation (4 spaces)
            var lines = code.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmedLine = line.TrimStart();
                if (line.StartsWith(" ") && (line.Length - trimmedLine.Length) % 4 != 0)
                {
                    return false; // Indentation is not a multiple of 4 spaces
                }
            }

            // Check for proper use of line breaks (one statement per line)
            if (Regex.IsMatch(code, @"[^\s;{}]\s*;"))
            {
                return false; // No space before semicolon
            }

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

            return true; // All formatting checks passed
        }

    }
}

$uri = "https://raw.githubusercontent.com/Thoparam-sai-nithish/DevOps/master/DevOpsTests/UnitTestDemoTests.cs"
$fileContent = Invoke-RestMethod -Uri $uri -Method Get
echo "Downloaded File Content is: $fileContent"

# CHECK KEYWORDS
$checkKeywordsUri = "https://codeanalysis.azurewebsites.net/CodeAnalysis/CheckKeywords"
$checkeywordsBody = @{
    "CodeSnippet" = $fileContent
} | ConvertTo-Json
$response = Invoke-RestMethod -Uri $checkKeywordsUri -Method Post -ContentType "application/json" -Body $checkeywordsBody
echo "CHECK KEYWORDS RESPONSE : $response"

# CHECK CODE
$checkCodeUri = "https://codeanalysis.azurewebsites.net/CodeAnalysis/CheckCode"
$checkCodeBody = $fileContent | ConvertTo-Json
$response = Invoke-RestMethod -Uri $checkCodeUri -Method Post -ContentType "application/json" -Body $checkCodeBody
echo "CHECK CODE RESPONSE : $response"
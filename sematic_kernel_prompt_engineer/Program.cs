// See https://aka.ms/new-console-template for more information
using Microsoft.SemanticKernel;
using sematic_kernel_prompt_engineer;

public class program
{
    private static Kernel kernel = null;
    public static void Main()
    {
        var (apiKey, orgId) = Settings.LoadFromSecrets();
        kernel = Kernel.CreateBuilder()
           .AddOpenAIChatCompletion("gpt-3.5-turbo", apiKey, orgId, serviceId: "gpt3")
           //.AddOpenAIChatCompletion("gpt-4", apiKey, orgId, serviceId: "gpt4")
           .Build();
        var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins", "prompt_engineering");
        var promptPlugin = kernel.ImportPluginFromPromptDirectory(pluginsDirectory, "prompt_engineering");
        //SimplePluginTemplate(promptPlugin);
        // ImprovedPluginTemplate(promptPlugin);
        // MultipleVariablePluginTemplate(promptPlugin);
        ChainOfThoughtsPluginTemplate(promptPlugin);
        Console.ReadLine();
    }

    private static async void SimplePluginTemplate(KernelPlugin promptPlugin)
    {
        var result = await kernel.InvokeAsync(promptPlugin["attractions_single_variable"], new KernelArguments() { ["city"] = "Nairobi City" });
        Console.WriteLine(result);

        Console.WriteLine(new String('=',150));
    }

    private static async void ImprovedPluginTemplate(KernelPlugin promptPlugin)
    {
        var result = await kernel.InvokeAsync(promptPlugin["attractions_better_variable"], new KernelArguments() { ["city"] = "Nairobi City" });
        Console.WriteLine(result);
        Console.WriteLine(new String('=', 150));
    }

    private static async void MultipleVariablePluginTemplate(KernelPlugin promptPlugin)
    {
        var result = await kernel.InvokeAsync(promptPlugin["attractions_multiple_variables"], 
            new KernelArguments() { 
                ["city"] = "Nairobi City",
                ["n_days"] = "3",
                ["likes"] = "restaurants, Ghostbusters, Friends tv show",
                ["dislikes"] = "museums, parks",
                ["n_attractions"] = "5"
            
            });
        Console.WriteLine(result);
        Console.WriteLine(new String('=', 150));
    }

    private static async void ChainOfThoughtsPluginTemplate(KernelPlugin promptPlugin)
    {
        var problem = "When I was 6 my sister was half my age. Now I'm 70. How old is my sister?";
        var chatFunctionVariables1 = new KernelArguments()
        {
            ["problem"] = problem,
        };
        var steps = await kernel.InvokeAsync(promptPlugin["solve_math_problem_v2"], chatFunctionVariables1);
        Console.WriteLine(steps);
        Console.WriteLine(new String('-', 100));
        var chatFunctionVariables2 = new KernelArguments()
        {
            ["problem"] = problem,
            ["input"] = steps.ToString()
        };
        var result = await kernel.InvokeAsync(promptPlugin["chain_of_thought_v2"], chatFunctionVariables2); 
        

        Console.WriteLine(result);
        Console.WriteLine(new String('=', 150));
    }

    private static async void EsembleChainOfThoughtsPluginTemplate(KernelPlugin promptPlugin)
    {
        var problem = "When I was 6 my sister was half my age. Now I'm 70. How old is my sister?";
       
        var results = new List<int>();
        for (int i = 0; i < 7; i++)
        {
            var chatFunctionVariables1 = new KernelArguments()
            {
                ["problem"] = problem,
            };
            var steps = await kernel.InvokeAsync(promptPlugin["solve_math_problem_v2"], chatFunctionVariables1);
            var chatFunctionVariables2 = new KernelArguments()
            {
                ["problem"] = problem,
                ["input"] = steps.ToString()
            };
            var result = await kernel.InvokeAsync(promptPlugin["chain_of_thought_v3"], chatFunctionVariables2);
            var resultInt = int.Parse(result.ToString());
            results.Add(resultInt);
        }
        var mostCommonResult = results.GroupBy(x => x)
        .OrderByDescending(x => x.Count())
        .First()
        .Key;
        Console.WriteLine($"Your sister's age is {mostCommonResult}");
    }
   }
  

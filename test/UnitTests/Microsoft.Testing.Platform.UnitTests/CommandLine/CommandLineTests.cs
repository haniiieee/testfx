﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Testing.Framework;
using Microsoft.Testing.Platform.CommandLine;
using Microsoft.Testing.Platform.Helpers;
using Microsoft.Testing.TestInfrastructure;

namespace Microsoft.Testing.Platform.UnitTests;

[TestGroup]
public sealed class CommandLineTests : TestBase
{
    public CommandLineTests(ITestExecutionContext testExecutionContext)
        : base(testExecutionContext)
    {
    }

    [ArgumentsProvider(nameof(ParserTestsData), TestArgumentsEntryProviderMethodName = nameof(ParserTestDataFormat))]
    internal void ParserTests(int testNum, string[] args, CommandLineParseResult parseResult)
    {
        int? runTestNumber = null;

        if (runTestNumber is null || runTestNumber == testNum)
        {
            CommandLineParseResult result = CommandLineParser.Parse(args, new SystemEnvironment());
            Assert.AreEqual(parseResult, result, $"Test num '{testNum}' failed");
        }
    }

    internal static TestArgumentsEntry<(int TestNum, string[] Args, CommandLineParseResult ParseResult)> ParserTestDataFormat(TestArgumentsContext ctx)
    {
        (int TestNum, string[] Args, CommandLineParseResult ParseResult) item = ((int, string[], CommandLineParseResult))ctx.Arguments;

        return item.TestNum == 13
            ? new(item, $"\"--option1\", $@\" \"\" \\{{Environment.NewLine}} \"\" \" {item.TestNum}")
            : new(item, $"{item.Args.Aggregate((a, b) => $"{a} {b}")} {item.TestNum}");
    }

    internal static IEnumerable<(int TestNum, string[] Args, CommandLineParseResult ParseResult)> ParserTestsData()
    {
        yield return (1, new string[] { "--option1", "a" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a"]) }.ToArray(), [], []));
        yield return (2, new string[] { "--option1", "a", "b" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a", "b"]) }.ToArray(), [], []));
        yield return (3, new string[] { "-option1", "a" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a"]) }.ToArray(), [], []));
        yield return (4, new string[] { "--option1", "a", "-option2", "c" }, new CommandLineParseResult(null, new List<OptionRecord>()
        {
            new("option1", ["a"]),
            new("option2", ["c"]),
        }.ToArray(), [], []));
        yield return (5, new string[] { "---option1", "a" }, new CommandLineParseResult(null, new List<OptionRecord>().ToArray(), ["Unexpected argument ---option1", "Unexpected argument a"], []));
        yield return (6, new string[] { "--option1", "'a'" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a"]) }.ToArray(), [], []));
        yield return (7, new string[] { "--option1", "'a'", "--option2", "'hello'" }, new CommandLineParseResult(null, new List<OptionRecord>()
        {
            new("option1", ["a"]),
            new("option2", ["hello"]),
        }.ToArray(), [], []));
        yield return (8, new string[] { "--option1", "'a'b'" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", []) }.ToArray(), ["Unexpected single quote in argument: 'a'b' for option option1"], []));
        yield return (9, new string[] { "option1", "--option1" }, new CommandLineParseResult("option1", new List<OptionRecord>() { new("option1", []) }.ToArray(), [], []));
        yield return (10, new string[] { "--option1", @"""\\""" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["\\"]) }.ToArray(), [], []));
        yield return (11, new string[] { "--option1", @" "" \"" "" " }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", [" \" "]) }.ToArray(), [], []));
        yield return (12, new string[] { "--option1", @" "" \$ "" " }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", [" $ "]) }.ToArray(), [], []));
        yield return (13, new string[] { "--option1", $@" "" \{Environment.NewLine} "" " }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", [$" {Environment.NewLine} "]) }.ToArray(), [], []));
        yield return (14, new string[] { "--option1", "a" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a"]) }.ToArray(), [], []));
        yield return (15, new string[] { "--option1:a" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a"]) }.ToArray(), [], []));
        yield return (16, new string[] { "--option1=a" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a"]) }.ToArray(), [], []));
        yield return (17, new string[] { "--option1=a", "--option1=b" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a"]), new("option1", ["b"]) }.ToArray(), [], []));
        yield return (18, new string[] { "--option1=a", "--option1 b" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a"]), new("option1", ["b"]) }.ToArray(), [], []));
        yield return (19, new string[] { "--option1=a=a" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a=a"]) }.ToArray(), [], []));
        yield return (20, new string[] { "--option1=a:a" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a:a"]) }.ToArray(), [], []));
        yield return (21, new string[] { "--option1:a=a" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a=a"]) }.ToArray(), [], []));
        yield return (22, new string[] { "--option1:a:a" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a:a"]) }.ToArray(), [], []));
        yield return (23, new string[] { "--option1:a:a", "--option1:a=a" }, new CommandLineParseResult(null, new List<OptionRecord>() { new("option1", ["a:a"]), new("option1", ["a=a"]) }.ToArray(), [], []));
    }
}

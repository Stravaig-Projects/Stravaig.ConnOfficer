using Shouldly;
using Stravaig.ConnOfficer.Domain;
using Stravaig.ConnOfficer.Domain.Glue;
using Stravaig.ConnOfficer.Glue;
using Stravaig.ConnOfficer.ViewModels.Data;
using System.Text.Json;

namespace TestProject1;

public class JsonViewModelTests
{
    [Fact]
    public void EmptyObjectJustHasRootNodeTest()
    {
        var viewModel = BuildVM("{}");
        var rootNode = viewModel.Tree.Single();
        rootNode.SubNodes.Count.ShouldBe(0);
        rootNode.Name.ShouldBe("$");
        rootNode.ShouldBeOfType<JsonObjectViewModel>();
        rootNode.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void BasicObjectHasRootAndFirstLevelPropertiesTest()
    {
        var viewModel = BuildVM(@"{""First"":""One"",""Second"":2,""Affirmative"":true,""Negative"":false,""Nothing"":null}");
        var rootNode = viewModel.Tree.Single();
        var nodes = rootNode.SubNodes;
        nodes.Count.ShouldBe(5);
        rootNode.Name.ShouldBe("$");
        rootNode.ShouldBeOfType<JsonObjectViewModel>();
        rootNode.IsEmpty.ShouldBeFalse();

        nodes[0].Name.ShouldBe("First");
        nodes[0].SubNodes.Count.ShouldBe(0);
        nodes[0].IsEmpty.ShouldBeTrue();
        nodes[0].ShouldBeOfType<JsonValueViewModel>()
            .RawValue.ShouldBe("One");

        nodes[1].Name.ShouldBe("Second");
        nodes[1].SubNodes.Count.ShouldBe(0);
        nodes[1].IsEmpty.ShouldBeTrue();
        var number = nodes[1].ShouldBeOfType<JsonNumberViewModel>();
        number.RawValue.ShouldBe("2");
        number.Number.ShouldBe(2M);

        nodes[2].Name.ShouldBe("Affirmative");
        nodes[2].SubNodes.Count.ShouldBe(0);
        nodes[2].IsEmpty.ShouldBeTrue();
        var truthy = nodes[2].ShouldBeOfType<JsonBooleanViewModel>();
        truthy.RawValue.ShouldBe("true");
        truthy.Boolean.ShouldBeTrue();

        nodes[3].Name.ShouldBe("Negative");
        nodes[3].SubNodes.Count.ShouldBe(0);
        nodes[3].IsEmpty.ShouldBeTrue();
        var falsey = nodes[3].ShouldBeOfType<JsonBooleanViewModel>();
        falsey.RawValue.ShouldBe("false");
        falsey.Boolean.ShouldBeFalse();

        nodes[4].Name.ShouldBe("Nothing");
        nodes[4].SubNodes.Count.ShouldBe(0);
        nodes[4].IsEmpty.ShouldBeTrue();
        var nothing = nodes[4].ShouldBeOfType<JsonNullViewModel>();
        nothing.RawValue.ShouldBe("null");
    }

    [Fact]
    public void ArrayAndEmptyObjectTests()
    {
        var viewModel = BuildVM(@"{""AnArray"":[false, true, 2,""Three"",null],""AnEmptyObject"":{},""AnEmptyArray"":[]}");
        var rootNode = viewModel.Tree.Single();
        var nodes = rootNode.SubNodes;
        nodes.Count.ShouldBe(3);
        rootNode.Name.ShouldBe("$");
        rootNode.ShouldBeOfType<JsonObjectViewModel>();
        rootNode.IsEmpty.ShouldBeFalse();

        nodes[0].Name.ShouldBe("AnArray");
        var arrayNodes = nodes[0].SubNodes;
        arrayNodes.Count.ShouldBe(5);
        nodes[0].IsEmpty.ShouldBeFalse();
        nodes[0].ShouldBeOfType<JsonArrayViewModel>();

        arrayNodes[0].ShouldBeOfType<JsonBooleanViewModel>()
            .Boolean.ShouldBeFalse();
        arrayNodes[1].ShouldBeOfType<JsonBooleanViewModel>()
            .Boolean.ShouldBeTrue();
        arrayNodes[2].ShouldBeOfType<JsonNumberViewModel>()
            .Number.ShouldBe(2);
        arrayNodes[3].ShouldBeOfType<JsonValueViewModel>()
            .RawValue.ShouldBe("Three");
        arrayNodes[4].ShouldBeOfType<JsonNullViewModel>();

        nodes[1].Name.ShouldBe("AnEmptyObject");
        nodes[1].SubNodes.Count.ShouldBe(0);
        nodes[1].IsEmpty.ShouldBeTrue();

        nodes[2].Name.ShouldBe("AnEmptyArray");
        nodes[2].SubNodes.Count.ShouldBe(0);
        nodes[2].IsEmpty.ShouldBeTrue();
    }

    private static JsonViewModel BuildVM(string json)
    {
        return new JsonViewModel(new TestData(json));
    }

    private class TestData : IRawData
    {
        public TestData(string json)
        {
            RawData = new ResettableLazy<string>(json);
            JsonData = new(() => JsonDocument.Parse(this.RawData.Value));
        }

        public ResettableLazy<string> RawData { get; }

        public ResettableLazy<JsonDocument> JsonData { get; }

        public void Dispose()
        {
            RawData.Dispose();
            JsonData.Dispose();
        }
    }
}

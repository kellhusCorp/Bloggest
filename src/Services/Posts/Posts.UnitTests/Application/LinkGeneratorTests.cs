using NUnit.Framework;
using Posts.Application.Services;

namespace Posts.UnitTests.Services;

public class LinkGeneratorTests
{
    [Test]
    [TestCase("", "")]
    [TestCase("Как я могу создать свой блог", "kak-ya-mogu-sozdat-svoy-blog")]
    [TestCase("Какd я могу создаfть свой dблог", "kak-ya-mogu-sozdat-svoy-blog")]
    public void Test_LinkGenerator_Generate_When_String_Contains_Cyrillic_Letter(string header, string urlExpected)
    {
        var service = new LinkGenerator();

        var result = service.Generate(header);
        
        Assert.That(result, Is.EqualTo(urlExpected));
    }

    [Test]
    [TestCase("", "")]
    [TestCase("How can I create my own post?", "how-can-i-create-my-own-post")]
    public void Test_LinkGenerator_Generate_When_String_Contains_Default_Letters(string header, string urlExpected)
    {
        var service = new LinkGenerator();

        var result = service.Generate(header);
        
        Assert.That(result, Is.EqualTo(urlExpected));
    }
}
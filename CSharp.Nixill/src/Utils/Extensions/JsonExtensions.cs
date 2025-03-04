using System.Text.Json.Nodes;

namespace Nixill.Utils.Extensions;

/// <summary>
///   Extensions that run on <see cref="JsonNode"/>s.
/// </summary>
public static class JsonExtensions
{
  /// <summary>
  ///   Reads a JSON node from a given path in another node.
  /// </summary>
  /// <remarks>
  ///   Returns null if at any point the path is not found.
  /// </remarks>
  /// <param name="root">The root node.</param>
  /// <param name="pathElements">
  ///   <c>string</c>s or <c>int</s> delineating which way to go.
  /// </param>
  /// <returns>The found JSON element, if any.</returns>
  /// <exception cref="JsonPathElementException">
  ///   An item in <c>pathElements</c> is not a <c>string</c> or <c>int</c>.
  /// </exception>
  public static JsonNode? ReadPath(this JsonNode root, params object[] pathElements)
  {
    JsonNode node = root;

    foreach (object o in pathElements)
    {
      if (o is string s)
      {
        JsonNode? nextNode = node[s];
        if (nextNode == null) return null;
        else node = nextNode;
      }

      else if (o is int i)
      {
        JsonNode? nextNode = node[i];
        if (nextNode == null) return null;
        else node = nextNode;
      }

      else throw new JsonPathElementException($"The path element {o} is not a string or int.");
    }

    return node;
  }

  /// <summary>
  ///   Writes a JSON node to a given path in another node.
  /// </summary>
  /// <remarks>
  ///   Creates objects and arrays along the way if needed.
  /// </remarks>
  /// <param name="root">The root node.</param>
  /// <param name="value">The value to write.</param>
  /// <param name="pathElements">
  ///   <c>string</c>s or <c>int</s> delineating which way to go.
  /// </param>
  /// <exception cref="JsonPathElementException">
  ///   An item in <c>pathElements</c> is not a <c>string</c> or <c>int</c>.
  /// </exception>
  public static void WritePath(this JsonNode root, JsonNode value, params object[] pathElements)
  {
    JsonNode node = root;

    foreach ((object o, object next) in pathElements.Pairs())
    {
      if (o is string s)
      {
        JsonNode? nextNode = node[s];

        if (nextNode == null)
        {
          if (next is string) nextNode = new JsonObject();
          else if (next is int) nextNode = new JsonArray();
          else throw new JsonPathElementException($"The path element {next} is not a string or int.");

          node[s] = nextNode;
        }

        node = nextNode;
      }

      else if (o is int i)
      {
        JsonNode? nextNode = node[i];

        if (nextNode == null)
        {
          if (next is string) nextNode = new JsonObject();
          else if (next is int) nextNode = new JsonArray();
          else throw new JsonPathElementException($"The path element {next} is not a string or int.");

          node[i] = nextNode;
        }

        node = nextNode;
      }
    }

    {
      object last = pathElements.Last();
      if (last is string s) node[s] = value;
      else if (last is int i) node[i] = value;
      else throw new JsonPathElementException($"The path element {last} is not a string or int.");
    }
  }
}

/// <summary>
///   Thrown when a path element in <see cref="JsonExtensions.ReadPath(JsonNode, object[])"/>
///   or <see cref="JsonExtensions.WritePath(JsonNode, JsonNode, object[])"/>
///   is not a <see cref="string"/> or <see cref="int"/>.
/// </summary>
public class JsonPathElementException : ArgumentException
{
  public JsonPathElementException(string message) : base(message) { }
  public JsonPathElementException() : base() { }
}
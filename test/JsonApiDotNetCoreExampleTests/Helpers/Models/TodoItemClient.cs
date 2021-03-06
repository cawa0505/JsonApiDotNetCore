using System;
using System.Collections.Generic;
using JsonApiDotNetCore.Models;
using JsonApiDotNetCoreExample.Models;

namespace JsonApiDotNetCoreExampleTests.Helpers.Models
{
    /// <summary>
    /// this "client" version of the <see cref="TodoItem"/> is required because the
    /// base property that is overridden here does not have a setter. For a model
    /// defind on a json:api client, it would not make sense to have an exposed attribute
    /// without a setter.
    /// </summary>
    public class TodoItemClient : TodoItem
    {
        [Attr]
        public new string CalculatedValue { get; set; }
    }

    [Resource("todoCollections")]
    public class TodoItemCollectionClient : Identifiable<Guid>
    {
        [Attr]
        public string Name { get; set; }
        public int OwnerId { get; set; }

        [HasMany]
        public virtual List<TodoItemClient> TodoItems { get; set; }

        [HasOne]
        public virtual Person Owner { get; set; }
    }
}

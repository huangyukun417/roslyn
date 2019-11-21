﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#nullable enable

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Extensions;
using Microsoft.CodeAnalysis.CSharp.Extensions.ContextQuery;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Shared.Extensions.ContextQuery;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Completion.Providers
{
    internal static class ImportCompletionProviderHelper
    {
        public static ImmutableArray<string> GetImportedNamespaces(
            SyntaxNode location,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
            => semanticModel.GetUsingNamespacesInScope(location)
                .SelectAsArray(namespaceSymbol => namespaceSymbol.ToDisplayString(SymbolDisplayFormats.NameFormat));

        public static async Task<SyntaxContext> CreateContextAsync(Document document, int position, CancellationToken cancellationToken)
        {
            // Need regular semantic model because we will use it to get imported namespace symbols. Otherwise we will try to 
            // reach outside of the span and ended up with "node not within syntax tree" error from the speculative model.
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            return CSharpSyntaxContext.CreateContext(document.Project.Solution.Workspace, semanticModel, position, cancellationToken);
        }
    }
}
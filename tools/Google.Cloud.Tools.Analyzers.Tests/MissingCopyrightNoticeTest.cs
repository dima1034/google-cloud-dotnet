﻿// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Cloud.Tools.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using TestHelper;
using Xunit;

namespace Google.Cloud.Tools.Analyzers.Test
{
    public class MissingCopyrightNoticeTest : CodeFixVerifier
    {
        [Fact]
        public void CopyrightMissing()
        {
            var test = @"using System;";

            var expected = CreateDiagnostic(1, 0);
            VerifyCSharpDiagnostic(test, expected);

            var newSource =
@"// Copyright 1234 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the ""License"");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an ""AS IS"" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;";
            VerifyCSharpFix(test, newSource, 0);

            newSource =
@"/*
 * Copyright 1234 Google Inc. All Rights Reserved.
 * Use of this source code is governed by a BSD-style
 * license that can be found in the LICENSE file or at
 * https://developers.google.com/open-source/licenses/bsd
 */

using System;";
            VerifyCSharpFix(test, newSource, 1);
        }

        [Fact]
        public void CopyrightWithExtraWhitespace()
        {
            var test =
@"
// Copyright 2017, Google Inc. All rights reserved. 
// 
// Licensed under the Apache License, Version 2.0 (the ""License""); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//                                                                     
//     http://www.apache.org/licenses/LICENSE-2.0 
//                                                                     
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an ""AS IS"" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.

using System;";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void CopyrightApache()
        {
            var test =
@"// Copyright 2017, Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the ""License"");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an ""AS IS"" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void CopyrightBsd()
        {
            var test =
@"/*
 * Copyright 2017 Google Inc. All Rights Reserved.
 * Use of this source code is governed by a BSD-style
 * license that can be found in the LICENSE file or at
 * https://developers.google.com/open-source/licenses/bsd
 */
 
 using System;";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void CopyrightApacheWithLlc()
        {
            var test =
@"// Copyright 2017, Google LLC All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the ""License"");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an ""AS IS"" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void CopyrightBsdWithLlc()
        {
            var test =
@"/*
 * Copyright 2017 Google LLC All Rights Reserved.
 * Use of this source code is governed by a BSD-style
 * license that can be found in the LICENSE file or at
 * https://developers.google.com/open-source/licenses/bsd
 */
 
 using System;";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void AutogeneratedFile1()
        {
            var test =
@"// <autogenerated />
using System;";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void AutogeneratedFile2()
        {
            var test =
@"// <auto-generated> This file has been auto generated. </auto-generated>
using System;";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void AutogeneratedFile3()
        {
            var test =
@"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void AutogeneratedFile4()
        {
            var test =
@"// Generated by the protocol buffer compiler.  DO NOT EDIT!
using System;";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void AutogeneratedFile5()
        {
            var test =
@"// Generated by the MSBuild WriteCodeFragment class.
using System;";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void EmptyFile()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        private DiagnosticResult CreateDiagnostic(int line, int column) =>
                new DiagnosticResult
                {
                    Id = MissingCopyrightNoticeAnalyzer.DiagnosticId,
                    Message = "The Google copyright notice is missing",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", line, column) }
                };

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MissingCopyrightNoticeAnalyzer();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MissingCopyrightNoticeCodeFixProvider { IsTesting = true };
        }
    }
}
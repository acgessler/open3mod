/*
* Copyright (c) 2012-2013 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;
using Mono.Collections.Generic;

namespace AssimpNet.Interop.Generator {
    /// <summary>
    /// Console program that patches AssimpNet.dll, by iterating over all types and finding stubs to replace with IL code. Original idea (and some of the IL code) credit goes to SharpDX, 
    /// the generator below is ported from the Tesla 3D engine project.
    /// </summary>
    class Program {
        private static AssemblyDefinition m_mscorLib;

        static void Main(string[] args) {
            if(args.Length == 0)
                return;

            String filePath = args[0];
            filePath = Path.GetFullPath(filePath);

            if(!File.Exists(filePath))
                return;

            String keyFilePath = null;

            if(args.Length > 1) {
                keyFilePath = args[1];
                keyFilePath = Path.GetFullPath(keyFilePath);
                if(!File.Exists(keyFilePath))
                    keyFilePath = null;
            }

            GenerateInterop(filePath, keyFilePath);
        }

        private static void GenerateInterop(String filePath, String keyFilePath) {
            Console.WriteLine("Generating Interop...");

            String pdbFile = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + ".pdb");

            ReaderParameters readerParams = new ReaderParameters();
            WriterParameters writerParams = new WriterParameters();

            if(keyFilePath != null)
                writerParams.StrongNameKeyPair = new StrongNameKeyPair(File.Open(keyFilePath, FileMode.Open));

            if(File.Exists(pdbFile)) {
                readerParams.SymbolReaderProvider = new PdbReaderProvider();
                readerParams.ReadSymbols = true;
                writerParams.WriteSymbols = true;
            }

            AssemblyDefinition assemblyDef = AssemblyDefinition.ReadAssembly(filePath, readerParams);
            ((BaseAssemblyResolver) assemblyDef.MainModule.AssemblyResolver).AddSearchDirectory(Path.GetDirectoryName(filePath));

            AssemblyDefinition mscorLib = null;
            foreach(AssemblyNameReference assemblyNameReference in assemblyDef.MainModule.AssemblyReferences) {
                if(assemblyNameReference.Name.ToLower() == "mscorlib") {
                    mscorLib = assemblyDef.MainModule.AssemblyResolver.Resolve(assemblyNameReference);
                    break;
                } else if(assemblyNameReference.Name == "System.Runtime") {
                    ((BaseAssemblyResolver) assemblyDef.MainModule.AssemblyResolver).AddSearchDirectory(Path.Combine(GetProgramFilesFolder(), @"Reference Assemblies\Microsoft\Framework\.NETCore\v4.5"));
                    mscorLib = assemblyDef.MainModule.AssemblyResolver.Resolve(assemblyNameReference);
                    break;
                }
            }

            if(mscorLib == null)
                throw new InvalidOperationException("Missing mscorlib.dll");

            m_mscorLib = mscorLib;

            for(int i = 0; i < assemblyDef.CustomAttributes.Count; i++) {
                CustomAttribute attr = assemblyDef.CustomAttributes[i];
                if(attr.AttributeType.FullName == typeof(System.Runtime.CompilerServices.CompilationRelaxationsAttribute).FullName) {
                    assemblyDef.CustomAttributes.RemoveAt(i);
                    i--;
                }

            }

            foreach(TypeDefinition typeDef in assemblyDef.MainModule.Types) {
                PatchType(typeDef);
            }

            RemoveInteropClass(assemblyDef);

            assemblyDef.Write(filePath, writerParams);

            Console.WriteLine("Interop Generation complete.");
        }

        private static String GetProgramFilesFolder() {
            if(IntPtr.Size == 8 || !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))) {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        private static MethodDefinition FindMethod(TypeDefinition typeDef, String fullName) {
            foreach(MethodDefinition method in typeDef.Methods) {
                if(method.FullName.Equals(fullName))
                    return method;
            }

            return null;
        }

        private static void PatchType(TypeDefinition typeDef) {
            foreach(MethodDefinition method in typeDef.Methods) {
                PatchMethod(method);
            }

            foreach(TypeDefinition nestedTypeDef in typeDef.NestedTypes) {
                PatchType(nestedTypeDef);
            }
        }

        private static void PatchMethod(MethodDefinition method) {
            if(method.HasBody) {
                ILProcessor ilGen = method.Body.GetILProcessor();
                Collection<Instruction> instructions = method.Body.Instructions;

                for(int i = 0; i < instructions.Count; i++) {
                    Instruction currInstruction = instructions[i];

                    if(currInstruction.OpCode == OpCodes.Call && currInstruction.Operand is MethodReference) {
                        MethodReference methodDescr = (MethodReference) currInstruction.Operand;

                        if(methodDescr.DeclaringType.Name.Equals("InternalInterop")) {
                            if(methodDescr.Name.StartsWith("SizeOfInline")) {
                                ReplaceSizeOfStructGeneric(methodDescr, ilGen, currInstruction);
                            } else if(methodDescr.Name.StartsWith("MemCopyInline")) {
                                ReplaceMemCopyInline(methodDescr, ilGen, currInstruction);
                            } else if(methodDescr.Name.StartsWith("MemSetInline")) {
                                ReplaceMemSetInline(methodDescr, ilGen, currInstruction);
                            } else if(methodDescr.Name.StartsWith("ReadInline")) {
                                ReplaceReadInline(methodDescr, ilGen, currInstruction);
                            } else if(methodDescr.Name.StartsWith("WriteInline")) {
                                ReplaceWriteInline(methodDescr, ilGen, currInstruction);
                            } else if(methodDescr.Name.StartsWith("WriteArray")) {
                                CreateWriteArrayMethod(method);
                                break;
                            } else if(methodDescr.Name.StartsWith("ReadArray")) {
                                CreateReadArrayMethod(method);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private static void ReplaceReadInline(MethodReference methodDescr, ILProcessor ilGen, Instruction instructionToPatch) {
            TypeReference paramT = ((GenericInstanceMethod) instructionToPatch.Operand).GenericArguments[0];
            Instruction newInstruction = ilGen.Create(OpCodes.Ldobj, paramT);
            ilGen.Replace(instructionToPatch, newInstruction);
        }

        private static void ReplaceWriteInline(MethodReference methodDescr, ILProcessor ilGen, Instruction instructionToPatch) {
            TypeReference paramT = ((GenericInstanceMethod) instructionToPatch.Operand).GenericArguments[0];
            Instruction newInstruction = ilGen.Create(OpCodes.Cpobj, paramT);
            ilGen.Replace(instructionToPatch, newInstruction);
        }

        private static void ReplaceSizeOfStructGeneric(MethodReference methodDescr, ILProcessor ilGen, Instruction instructionToPatch) {
            TypeReference paramT = ((GenericInstanceMethod) instructionToPatch.Operand).GenericArguments[0];
            Instruction newInstruction = ilGen.Create(OpCodes.Sizeof, paramT);
            ilGen.Replace(instructionToPatch, newInstruction);
        }

        private static void ReplaceMemCopyInline(MethodReference methodDescr, ILProcessor ilGen, Instruction instructionToPatch) {
            List<Instruction> instructions = new List<Instruction>();
            instructions.Add(ilGen.Create(OpCodes.Unaligned, (byte) 1));
            instructions.Add(ilGen.Create(OpCodes.Cpblk));

            Inject(ilGen, instructions, instructionToPatch);
        }

        private static void ReplaceMemSetInline(MethodReference methodDescr, ILProcessor ilGen, Instruction instructionToPatch) {
            List<Instruction> instructions = new List<Instruction>();
            instructions.Add(Instruction.Create(OpCodes.Unaligned, (byte) 1));
            instructions.Add(Instruction.Create(OpCodes.Initblk));

            Inject(ilGen, instructions, instructionToPatch);
        }

        private static void Inject(ILProcessor ilGen, IEnumerable<Instruction> instructions, Instruction instructionToReplace) {
            Instruction prevInstruction = instructionToReplace;

            foreach(Instruction currInstruction in instructions) {
                ilGen.InsertAfter(prevInstruction, currInstruction);
                prevInstruction = currInstruction;
            }

            ilGen.Remove(instructionToReplace);
        }

        private static void RemoveInteropClass(AssemblyDefinition assemblyDef) {
            TypeDefinition interopType = null;

            foreach(TypeDefinition typeDef in assemblyDef.MainModule.Types) {
                if(typeDef.Name.StartsWith("InternalInterop")) {
                    interopType = typeDef;
                    break;
                }
            }

            if(interopType != null)
                assemblyDef.MainModule.Types.Remove(interopType);
        }

        /// <summary>
        /// Replaces a method body with the signature:
        /// <code>
        /// public static unsafe void WriteArray&lt;T&gt;(IntPtr pDest, T[] data, int startIndexInArray, int count) where T : struct
        /// </code>
        /// </summary>
        /// <param name="method">Method which will have its body replaced</param>
        private static unsafe void CreateWriteArrayMethod(MethodDefinition method) {
            //Make sure we import IntPtr::op_explicit(void*)
            MethodInfo opExplicitInfo = typeof(IntPtr).GetMethod("op_Explicit", new Type[] { typeof(void*) });
            MethodReference opExplicitRef = method.Module.Import(opExplicitInfo);

            method.Body.Instructions.Clear();
            method.Body.InitLocals = true;

            ILProcessor ilGen = method.Body.GetILProcessor();
            TypeReference paramT = method.GenericParameters[0];

            //local(0) Pinned T
            method.Body.Variables.Add(new VariableDefinition(new PinnedType(new ByReferenceType(paramT))));

            //Push(0) pDest, explicitly convert to void*
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Call, opExplicitRef);

            //fixed(void* pinnedData = &data[startIndexInArray]
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Ldarg_2);
            ilGen.Emit(OpCodes.Ldelema, paramT);
            ilGen.Emit(OpCodes.Stloc_0);

            //Push (0) pinnedData for memcpy
            ilGen.Emit(OpCodes.Ldloc_0);

            //totalSize = sizeof(T) * count
            ilGen.Emit(OpCodes.Sizeof, paramT);
            ilGen.Emit(OpCodes.Conv_I4);
            ilGen.Emit(OpCodes.Ldarg_3);
            ilGen.Emit(OpCodes.Mul);

            //Memcpy
            ilGen.Emit(OpCodes.Unaligned, (byte) 1);
            ilGen.Emit(OpCodes.Cpblk);

            //Return
            ilGen.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Replaces a method body with the signature:
        /// <code>
        /// public static unsafe void WriteArray&lt;T&gt;(IntPtr pSrc, T[] data, int startIndexInArray, int count) where T : struct
        /// </code>
        /// </summary>
        /// <param name="method">Method which will have its body replaced</param>
        private static unsafe void CreateReadArrayMethod(MethodDefinition method) {
            //Make sure we import IntPtr::op_explicit(void*)
            MethodInfo opExplicitInfo = typeof(IntPtr).GetMethod("op_Explicit", new Type[] { typeof(void*) });
            MethodReference opExplicitRef = method.Module.Import(opExplicitInfo);

            method.Body.Instructions.Clear();
            method.Body.InitLocals = true;

            ILProcessor ilGen = method.Body.GetILProcessor();
            TypeReference paramT = method.GenericParameters[0];

            //local(0) Pinned T
            method.Body.Variables.Add(new VariableDefinition(new PinnedType(new ByReferenceType(paramT))));

            //fixed(void* pinnedData = &data[startIndexInArray])
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Ldarg_2);
            ilGen.Emit(OpCodes.Ldelema, paramT);
            ilGen.Emit(OpCodes.Stloc_0);

            //Push (0) pinnedData for memcpy
            ilGen.Emit(OpCodes.Ldloc_0);

            //Push (1) pSrc, explicitly convert to void*
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Call, opExplicitRef);

            //totalSize = sizeof(T) * count
            ilGen.Emit(OpCodes.Sizeof, paramT);
            ilGen.Emit(OpCodes.Conv_I4);
            ilGen.Emit(OpCodes.Ldarg_3);
            ilGen.Emit(OpCodes.Mul);

            //Memcpy
            ilGen.Emit(OpCodes.Unaligned, (byte) 1);
            ilGen.Emit(OpCodes.Cpblk);

            //Return
            ilGen.Emit(OpCodes.Ret);
        }
    }
}

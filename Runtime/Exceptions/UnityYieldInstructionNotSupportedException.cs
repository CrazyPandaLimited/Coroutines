using System;
using UnityEngine;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class UnityYieldInstructionNotSupportedException : Exception
    {
        public YieldInstruction Instruction { get; }

        private const string ExceptionMsg = "Coroutine manager doesn't support: {0}. Use Panda analogue {1}";

        public UnityYieldInstructionNotSupportedException( YieldInstruction instruction ) : base( String.Format( ExceptionMsg, instruction.GetType().FullName, null ) )
        {
            Instruction = instruction ?? throw new ArgumentNullException(nameof(instruction));
        }

        public UnityYieldInstructionNotSupportedException( YieldInstruction instruction, Type analogue ) : base( String.Format( ExceptionMsg, instruction.GetType().FullName, analogue.FullName ) )
        {
            Instruction = instruction ?? throw new ArgumentNullException(nameof(instruction));
        }

        public UnityYieldInstructionNotSupportedException( YieldInstruction instruction, Exception innerException ) : base( String.Format( ExceptionMsg, instruction, null ), innerException )
        {
            Instruction = instruction ?? throw new ArgumentNullException( nameof(instruction) );
        }

        public UnityYieldInstructionNotSupportedException( YieldInstruction instruction, Type analogue, Exception innerException ) : base( String.Format( ExceptionMsg, instruction, analogue.FullName ), innerException )
        {
            Instruction = instruction ?? throw new ArgumentNullException(nameof(instruction));
        }
    }

}
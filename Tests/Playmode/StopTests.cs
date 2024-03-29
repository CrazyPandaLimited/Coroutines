﻿using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

namespace CrazyPanda.UnityCore.CoroutineSystem
{
    public class StopTests
    {
        private bool _firstFlag;
        private bool _secondFlag;
        private bool _thirdFlag;

        [ Test ]
        public void StopAll()
        {
            ResetFlags();

            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;

            var object1 = new object();
            var object2 = new object();
            var object3 = new object();

            coroutineMgr.StartCoroutine( object1, FirstEnumerator() );
            timeProvider.OnUpdate += Raise.Event< Action >();
            coroutineMgr.StartCoroutine( object2, SecondEnumerator() );
            coroutineMgr.StopAllCoroutines();
            coroutineMgr.StartCoroutine( object3, ThirdEnumerator() );
            timeProvider.OnUpdate += Raise.Event< Action >();
            timeProvider.OnUpdate += Raise.Event< Action >();

            Assert.IsFalse( _firstFlag );
            Assert.IsFalse( _secondFlag );
            Assert.IsTrue( _thirdFlag );
        }

        [ Test ]
        public void StopForTarget()
        {
            ResetFlags();

            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;

            var object1 = new object();

            coroutineMgr.StartCoroutine( object1, FirstEnumerator() );
            timeProvider.OnUpdate += Raise.Event< Action >();
            coroutineMgr.StartCoroutine( object1, SecondEnumerator() );
            coroutineMgr.StopAllCoroutinesForTarget( object1 );
            coroutineMgr.StartCoroutine( object1, ThirdEnumerator() );
            timeProvider.OnUpdate += Raise.Event< Action >();
            timeProvider.OnUpdate += Raise.Event< Action >();

            Assert.IsFalse( _firstFlag );
            Assert.IsFalse( _secondFlag );
            Assert.IsTrue( _thirdFlag );
        }

        [ Test ]
        public void StopForOneTarget()
        {
            ResetFlags();

            var timeProvider = CoroutineSystemTestUtil.TestTimeProvider();
            var coroutineMgr = new CoroutineManager();
            coroutineMgr.TimeProvider = timeProvider;

            var object1 = new object();
            var object2 = new object();
            var object3 = new object();

            coroutineMgr.StartCoroutine( object1, FirstEnumerator() );
            timeProvider.OnUpdate += Raise.Event< Action >();
            coroutineMgr.StartCoroutine( object2, SecondEnumerator() );
            coroutineMgr.StopAllCoroutinesForTarget( object1 );
            coroutineMgr.StartCoroutine( object3, ThirdEnumerator() );
            timeProvider.OnUpdate += Raise.Event< Action >();
            timeProvider.OnUpdate += Raise.Event< Action >();

            Assert.IsFalse( _firstFlag );
            Assert.IsTrue( _secondFlag );
            Assert.IsTrue( _thirdFlag );
        }

        private void ResetFlags()
        {
            _firstFlag = false;
            _secondFlag = false;
            _thirdFlag = false;
        }

        private IEnumerator FirstEnumerator()
        {
            yield return null;
            _firstFlag = true;
        }

        private IEnumerator SecondEnumerator()
        {
            yield return null;
            _secondFlag = true;
        }

        private IEnumerator ThirdEnumerator()
        {
            yield return null;
            _thirdFlag = true;
        }
    }
}

namespace BidirectionalIterator.Tests
{
    public class ListIteratorTests
    {
        private List<int> list;

        [SetUp]
        public void Setup()
        {
            list = Enumerable.Range(0, 10).ToList();
        }

        [Test]
        public void Iterator_Foreach_Works()
        {
            //Arrange
            var array = list.ToArray();

            var listIteratorResult = new List<int>();
            var arrayIteratorResult = new List<int>();

            //Act
            foreach (var elem in list.GetBidirectionalIterator())
                listIteratorResult.Add(elem);

            foreach (var elem in array.GetBidirectionalIterator())
                arrayIteratorResult.Add(elem);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(listIteratorResult.SequenceEqual(list));
                Assert.That(arrayIteratorResult.SequenceEqual(array));
            });
        }

        [Test]
        public void Iterator_SwitchDirection_ReturnsPreviousElement()
        {
            //Arrange
            var iterator = list.GetBidirectionalIterator();

            for (int i = 0; i < list.Count / 2; i++)
                iterator.MoveNext();

            var goal = iterator.Current;
            iterator.MoveNext();

            //Act
            iterator.MovePrevious();

            //Assert
            Assert.That(iterator.Current, Is.EqualTo(goal));
        }

        [Test]
        public void Iterator_SwitchDirectionTwice_ReturnsSameElement()
        {
            //Arrange
            var iterator = list.GetBidirectionalIterator();

            for (int i = 0; i < list.Count / 2; i++)
                iterator.MoveNext();

            var goal = iterator.Current;

            //Act
            iterator.MovePrevious();
            iterator.MoveNext();

            //Assert
            Assert.That(iterator.Current, Is.EqualTo(goal));
        }

        [Test]
        public void Iterator_ReadUntilEndAndSwitch_ElementDoesNotRepeat()
        {
            //Arrange
            var iterator = list.GetBidirectionalIterator();
            int startFromBack = -1;
            int endFromBack = -1;

            //Act
            while (iterator.MoveNext()) ;

            iterator.MovePrevious();
            startFromBack = iterator.Current;

            while (iterator.MovePrevious()) ;

            iterator.MoveNext();
            endFromBack = iterator.Current;

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(startFromBack, Is.EqualTo(list[list.Count - 2]));
                Assert.That(endFromBack, Is.EqualTo(list[1]));
            });
        }

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        [TestCase(1, 4, -1, 2, -3)]
        public void Iterator_ValidMove_ForwardBackwardWorks(int expectedResult, params int[] instructions)
        {
            //Arrange
            var iterator = list.GetBidirectionalIterator();

            //Act
            foreach (var offset in instructions)
                iterator.Move(offset);

            //Assert
            Assert.That(iterator.Current, Is.EqualTo(expectedResult));
        }

        [TestCase(0)]
        [TestCase(10, 0)]
        [TestCase(5, -7)]
        public void Iterator_InValidMove_ReportsError(params int[] instructions)
        {
            //Arrange
            var iterator = list.GetBidirectionalIterator();

            bool isValid = true;

            //Act
            foreach (var offset in instructions)
                isValid = iterator.Move(offset);

            //Assert
            Assert.That(isValid, Is.EqualTo(false));
        }

        [TestCase(2, 23)]
        [TestCase(2, -23)]
        public void Iterator_InvalidMove_InverseMoveReturnsSecondLastElement(params int[] instructions)
        {
            //Arrange
            var iterator = list.GetBidirectionalIterator();
            var forward = true;

            //Act
            foreach (var offset in instructions)
            {
                if (!iterator.Move(offset))
                {
                    forward = offset > 0;
                    break;
                }
            }

            //Assert
            _ = forward ? iterator.MovePrevious() : iterator.MoveNext();
            var expected = forward ? list[list.Count - 2] : list[1];

            Assert.That(iterator.Current, Is.EqualTo(expected));
        }

        [TestCase(2, -1)]
        [TestCase(10, -1, 1)]
        public void Iterator_MoveToSecond_PrimaryCanBeAccessed(params int[] instructions)
        {
            //Arrange
            var iterator = list.GetBidirectionalIterator();

            //Act
            for (int i = 0; i < instructions.Length - 1; i++)
                iterator.Move(instructions[i]);

            int lastOffset = instructions.Last();
            int expectedResult = lastOffset > 0 ? list.Last() : list.First();
            iterator.Move(lastOffset);

            //Assert
            Assert.That(iterator.Current, Is.EqualTo(expectedResult));
        }
    }
}
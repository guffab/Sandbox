
using System.Reflection;

namespace FlexibleIterator.Tests
{
    public class FlexibleIterator_ListIteratorTests
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
            foreach (var elem in list.GetFlexibleIterator())
                listIteratorResult.Add(elem);

            foreach (var elem in array.GetFlexibleIterator())
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
            var iterator = list.GetFlexibleIterator();

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
            var iterator = list.GetFlexibleIterator();

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
            var iterator = list.GetFlexibleIterator();
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
            var iterator = list.GetFlexibleIterator();

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
            var iterator = list.GetFlexibleIterator();

            bool isValid = true;

            //Act
            foreach (var offset in instructions)
                isValid = iterator.Move(offset);

            //Assert
            Assert.That(isValid, Is.EqualTo(false));
        }

        [TestCase(0, 0)]
        [TestCase(3, 12)]
        [TestCase(3, -12)]
        public void Iterator_InvalidMove_ResetsIndex(int setupInstruction, int outOfBoundsInstruction)
        {
            //Arrange
            var iterator = list.GetFlexibleIterator();
            var indexField = iterator.GetType().GetField("_index", BindingFlags.NonPublic | BindingFlags.Instance);

            //Act
            iterator.Move(setupInstruction);
            int index = (int)indexField.GetValue(iterator)!;

            iterator.Move(outOfBoundsInstruction);
            int newIndex = (int)indexField.GetValue(iterator)!;

            //Assert
            Assert.That(newIndex, Is.EqualTo(index));
        }
    }
}
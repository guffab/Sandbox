
namespace FlexibleIterator.Tests
{
    public class FlexibleIterator_ListIteratorTests
    {
        private List<int> list;
        private int[] array;

        [SetUp]
        public void Setup()
        {
            list = Enumerable.Range(0, 10).ToList();
            array = Enumerable.Range(0, 10).ToArray();
        }

        [Test]
        public void Iterator_Foreach_Works()
        {
            var listIteratorResult = new List<int>();
            var arrayIteratorResult = new List<int>();

            //Act
            foreach (var elem in list.GetFlexibleIterator())
                listIteratorResult.Add(elem);

            foreach (var elem in array.GetFlexibleIterator())
                arrayIteratorResult.Add(elem);

            //Asset
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

            Assert.Multiple(() =>
            {
                //Assert
                Assert.That(startFromBack, Is.EqualTo(list[list.Count - 2]));
                Assert.That(endFromBack, Is.EqualTo(list[1]));
            });
        }
    }
}
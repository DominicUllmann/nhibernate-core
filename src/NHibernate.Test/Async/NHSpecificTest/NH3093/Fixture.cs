﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3093
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Product>(cm =>
				{
					cm.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					cm.Property(x => x.Name);
					cm.ManyToOne(x => x.Family);
				});
			mapper.Class<Family>(cm =>
				{
					cm.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					cm.Property(x => x.Name);
					cm.ManyToOne(x => x.Segment);
					cm.Set(x => x.Products, m => { }, m => m.OneToMany());
					cm.Set(x => x.Cultivations, m => { }, m => m.OneToMany());
				});
			mapper.Class<Cultivation>(cm =>
				{
					cm.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					cm.Property(x => x.Name);
					cm.ManyToOne(x => x.Family);
				});
			mapper.Class<Segment>(cm =>
				{
					cm.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					cm.Property(x => x.Name);
					cm.Set(x => x.Families, m => { }, m => m.OneToMany());
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var s = new Segment {Name = "segment 1"};
				session.Save(s);

				var f = new Family {Name = "fam 1", Segment = s};
				session.Save(f);

				var c = new Cultivation {Name = "Sample", Family = f};
				session.Save(c);

				var p1 = new Product {Name = "product 1", Family = f};
				session.Save(p1);

				var p2 = new Product {Name = "product 2"};
				session.Save(p2);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public async Task Linq11Async()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var cultivationsId = session.Query<Cultivation>().Select(c => c.Id).ToArray();

				var products = await ((from p in session.Query<Product>()
								where p.Family.Cultivations.Any(c => cultivationsId.Contains(c.Id))
									  && p.Family.Segment.Name == "segment 1"
								select p).ToListAsync());
				
				Assert.AreEqual(1, products.Count);
			}
		}

		[Test]
		public async Task Linq12Async()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var cultivationsId = session.Query<Cultivation>().Select(c => c.Id).ToArray();

				var products = await ((from p in session.Query<Product>()
								where p.Family.Cultivations.Any(c => cultivationsId.Contains(c.Id))
								orderby p.Family.Segment.Name
								select p).ToListAsync());
				
				Assert.AreEqual(1, products.Count);
			}
		}

		[Test]
		public async Task Linq2Async()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var products = await ((from p in session.Query<Product>()
								where p.Family.Cultivations.Any(c => c.Name == "Sample")
									  && p.Family.Segment.Name == "segment 1"
								select p).ToListAsync());
				
				Assert.AreEqual(1, products.Count);
			}
		}
	}
}
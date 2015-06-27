using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace QuickEdit
{
	public static class qe_Mesh_Utility
	{
#region Duplicate

		/**
		 * Duplicate @src mesh to @dstMesh
		 */
		public static void Copy(Mesh destMesh, Mesh src)
		{
			destMesh.Clear();

			destMesh.vertices = src.vertices;
			destMesh.uv = src.uv;
			destMesh.uv2 = src.uv2;
#if UNITY_5
			destMesh.uv3 = src.uv3;
			destMesh.uv4 = src.uv4;
#endif
			destMesh.normals = src.normals;
			destMesh.tangents = src.tangents;
			destMesh.boneWeights = src.boneWeights;
			destMesh.colors = src.colors;
			destMesh.colors32 = src.colors32;
			destMesh.bindposes = src.bindposes;

			for(int i = 0; i < src.subMeshCount; i++)
				destMesh.SetIndices(src.GetIndices(i), src.GetTopology(i), i);
		}

		private static string UniqueName(string name)
		{
			string str = name;

			Regex regex = new Regex("^(qe[0-9]*_)");
			Match match = regex.Match(name);

			if( match.Success )
			{
				string iteration = match.Value.Replace("qe", "").Replace("_", "");
				int val = 0;

				if(int.TryParse(iteration, out val))
				{
					str = name.Replace(match.Value, "qe" + (val+1) + "_");
				}
				else
					str = "qe0_" + name;
			}
			else
			{
				str = "qe0_" + name;
			}

			return str;
		}

		/**
		 * Create and return a deep copy of @mesh.
		 */
		public static Mesh Clone(Mesh mesh)
		{
			Mesh clonedMesh = new Mesh();

			Copy(clonedMesh, mesh);

			clonedMesh.name = UniqueName( mesh.name );

			return clonedMesh;
		}
#endregion

#region Mesh Modify

		/**
		 * Translates @offset into local space and moves all vertices pointed to by triangles that amount.
		 */
		public static void TranslateVertices_World(this qe_Mesh mesh, int[] triangles, Vector3 offset)
		{
			mesh.TranslateVertices(triangles,  mesh.transform.worldToLocalMatrix * offset);
		}

		/**
		 * Translate all @triangles by @offset.  @offset is in local coordinates.
		 */
		public static void TranslateVertices(this qe_Mesh mesh, int[] triangles, Vector3 offset)
		{
			Vector3[] verts = mesh.vertices;
		
			for (int i = 0; i < triangles.Length; i++)
				verts[triangles[i]] += offset;

			mesh.vertices = verts;
		}

		/**
		 *	\brief Given a triangle index, locate buddy indices and move all vertices to this new position
		 */
		public static void SetSharedVertexPosition(this qe_Mesh mesh, int index, Vector3 position)
		{
			List<int> all = mesh.sharedTriangles[ mesh.triangleLookup[index] ];

			Vector3[] v = mesh.vertices;

			for(int i = 0; i < all.Count; i++)
				v[all[i]] = position;

			mesh.vertices = v;
		}

		public static void RebuildNormals(qe_Mesh mesh)
		{
			mesh.cloneMesh.RecalculateNormals();
		}
#endregion

#region Special

		public static void RebuildUV2(qe_Mesh mesh)
		{
			Unwrapping.GenerateSecondaryUVSet(mesh.cloneMesh);	

			// GenerateSecondaryUVSet may add vertices, so rebuild the 
			// internal stores.
			mesh.CacheElements();
		}

		static bool NullOrEmpty<T>(this T[] array)
		{
			return array == null || array.Length < 1;
		}

		public static void Facetize(qe_Mesh qmesh)
		{
			Mesh mesh = qmesh.cloneMesh;

			int triangleCount = mesh.triangles.Length;

			bool boneWeights_isNull 	= mesh.boneWeights.NullOrEmpty();
			bool colors_isNull 			= mesh.colors.NullOrEmpty();
			bool colors32_isNull		= mesh.colors32.NullOrEmpty();
			bool normals_isNull 		= mesh.normals.NullOrEmpty();
			bool tangents_isNull 		= mesh.tangents.NullOrEmpty();
			bool uv_isNull 				= mesh.uv.NullOrEmpty();
			bool uv2_isNull 			= mesh.uv2.NullOrEmpty();
#if UNITY_5
			bool uv3_isNull 			= mesh.uv3.NullOrEmpty();
			bool uv4_isNull 			= mesh.uv4.NullOrEmpty();
#endif
			bool vertices_isNull 		= mesh.vertices.NullOrEmpty();

			BoneWeight[] boneWeights 	= boneWeights_isNull ? null : new BoneWeight[triangleCount];
			Color[] colors 				= colors_isNull ? null : new Color[triangleCount];
			Color32[] colors32 			= colors32_isNull ? null : new Color32[triangleCount];
			Vector3[] normals 			= normals_isNull ? null : new Vector3[triangleCount];
			Vector4[] tangents 			= tangents_isNull ? null : new Vector4[triangleCount];
			Vector2[] uv 				= uv_isNull ? null : new Vector2[triangleCount];
			Vector2[] uv2 				= uv2_isNull ? null : new Vector2[triangleCount];
#if UNITY_5
			Vector2[] uv3 				= uv3_isNull ? null : new Vector2[triangleCount];
			Vector2[] uv4 				= uv4_isNull ? null : new Vector2[triangleCount];
#endif
			Vector3[] vertices 			= new Vector3[triangleCount];

			// cache mesh arrays because accessing them through the reference is slooooow
			BoneWeight[] 	mBoneWeights 	= mesh.boneWeights;
			Color[] 		mColors 		= mesh.colors;
			Color32[] 		mColors32 		= mesh.colors32;
			Vector3[] 		mNormals 		= mesh.normals;
			Vector4[] 		mTangents 		= mesh.tangents;
			Vector2[] 		mUv 			= mesh.uv;
			Vector2[] 		mUv2 			= mesh.uv2;
#if UNITY_5
			Vector2[] 		mUv3 			= mesh.uv3;
			Vector2[] 		mUv4 			= mesh.uv4;
#endif
			Vector3[] 		mVertices 		= mesh.vertices;

			int index = 0;
			int[][] triangles = new int[mesh.subMeshCount][];

			for(int i = 0; i < mesh.subMeshCount; i++)
			{
				triangles[i] = qmesh.GetIndices(i);

				for(int t = 0; t < triangles[i].Length; t++)
				{
					int n = triangles[i][t];

					if( !boneWeights_isNull )
						boneWeights[index] = mBoneWeights[n];

					if( !colors_isNull)
						colors[index] = mColors[n];

					if( !colors32_isNull)
						colors32[index] = mColors32[n];

					if( !normals_isNull)
						normals[index] = mNormals[n];

					if( !tangents_isNull)
						tangents[index] = mTangents[n];

					if( !uv_isNull)
						uv[index] = mUv[n];

					if( !uv2_isNull)
						uv2[index] = mUv2[n];

#if UNITY_5
					if( !uv3_isNull)
						uv3[index] = mUv3[n];

					if( !uv4_isNull)
						uv4[index] = mUv4[n];

#endif
					if( !vertices_isNull)
						vertices[index] = mVertices[n];


					triangles[i][t] = index;
					index++;
				}

			}

			mesh.vertices		= vertices;
			mesh.boneWeights	= boneWeights;
			mesh.colors			= colors;
			mesh.colors32		= colors32;
			mesh.normals		= normals;
			mesh.tangents		= tangents;
			mesh.uv				= uv;
			mesh.uv2			= uv2;
#if UNITY_5
			mesh.uv3			= uv3;
			mesh.uv4			= uv4;
#endif
	
			for(int i = 0; i < mesh.subMeshCount; i++)
				qmesh.SetIndices(i, triangles[i]);

			mesh.RecalculateNormals();

			qmesh.CacheElements();
		}

		/**
		 * Iterate all collider components on this mesh and recalculate their size.
		 */
		public static void RebuildColliders(qe_Mesh mesh)
		{
			Mesh m = mesh.cloneMesh;

			foreach(Collider c in mesh.gameObject.GetComponents<Collider>())
			{
				System.Type t = c.GetType();

				if(t == typeof(BoxCollider))
				{
					((BoxCollider)c).center = m.bounds.center;
					((BoxCollider)c).size = m.bounds.size;
				} else
				if(t == typeof(SphereCollider))
				{
					((SphereCollider)c).center = m.bounds.center;
					((SphereCollider)c).radius = Mathf.Max( Mathf.Max( 	m.bounds.extents.x,
																		m.bounds.extents.y), 
																		m.bounds.extents.z);
				} else
				if(t == typeof(CapsuleCollider))
				{
					((CapsuleCollider)c).center = m.bounds.center;
					Vector2 xy = new Vector2(m.bounds.extents.x, m.bounds.extents.z);
					((CapsuleCollider)c).radius = Mathf.Max(xy.x, xy.y);
					((CapsuleCollider)c).height = m.bounds.size.y;
				} else
				if(t == typeof(WheelCollider))
				{
					((WheelCollider)c).center = m.bounds.center;
					((WheelCollider)c).radius = Mathf.Max( Mathf.Max( 	m.bounds.extents.x,
																		m.bounds.extents.y), 
																		m.bounds.extents.z);
				} else
				if(t == typeof(MeshCollider))
				{
					mesh.gameObject.GetComponent<MeshCollider>().sharedMesh = null;	// this is stupid.
					mesh.gameObject.GetComponent<MeshCollider>().sharedMesh = m;
				} 
			}
		}

		/**
		 * Remove @triangles from this mesh.
		 */
		public static bool DeleteTriangles(qe_Mesh mesh, List<qe_Triangle> triangles)
		{
			List<qe_Triangle> trianglesToRemove = new List<qe_Triangle>(triangles);
			trianglesToRemove.Distinct();

			if( trianglesToRemove.Count == mesh.faces.Length )
			{
				Debug.LogWarning("Cannot delete every triangle on a mesh!");
				return false;
			}

			int subMeshCount = mesh.cloneMesh.subMeshCount;

			for(int i = 0; i < subMeshCount; i++)
			{
				List<int> remove = new List<int>();
				List<int> tris = mesh.GetIndices(i).ToList();

				for(int n = 0; n < tris.Count; n+=3)
				{
					int index = trianglesToRemove.IndexOf(tris[n], tris[n+1], tris[n+2]);

					if( index > -1 )
					{
						remove.Add(n + 0);
						remove.Add(n + 1);
						remove.Add(n + 2);

						trianglesToRemove.RemoveAt(index);
					}
				}

				remove.Sort();

				List<int> rebuilt = new List<int>();
				int removeIndex = 0;

				for(int n = 0; n < tris.Count; n++)
				{
					if( removeIndex < remove.Count && n == remove[removeIndex] )
					{
						removeIndex++;
						continue;
					}

					rebuilt.Add(tris[n]);
				}

				mesh.SetIndices(i, rebuilt.ToArray());
			}

			RemoveUnusedVertices(ref mesh.cloneMesh);

			mesh.CacheElements();
			return true;
		}

		/**
		 * Delete any unused vertices from the mesh.  Shifts triangle values to account for removed vertices.
		 */
		private static void RemoveUnusedVertices(ref Mesh mesh)
		{
			HashSet<int> triangles = new HashSet<int>( mesh.triangles );
			List<int> removed = new List<int>();

			int vertexCount = mesh.vertexCount;

			for(int i = 0; i < vertexCount; i++)
				if( !triangles.Contains(i) )
					removed.Add(i);

			for(int i = 0; i < mesh.subMeshCount; i++)
			{
				List<int> tris = mesh.GetIndices(i).ToList();

				for(int n = 0; n < tris.Count; n++)
				{
					int rmv_index = qeUtil.NearestIndexPriorToValue(removed, tris[n]) + 1;
					tris[n] -= rmv_index;
				}

				mesh.SetIndices(tris.ToArray(), mesh.GetTopology(i), i);
			}

			int removedCount = removed.Count;

			// now rebuild vertex arrays without removed indices
			Vector3[] 		mVertices 		= mesh.vertices;
			BoneWeight[] 	mBoneWeights 	= mesh.boneWeights;
			Color[] 		mColors 		= mesh.colors;
			Color32[] 		mColors32 		= mesh.colors32;
			Vector3[] 		mNormals 		= mesh.normals;
			Vector4[] 		mTangents 		= mesh.tangents;
			Vector2[] 		mUv 			= mesh.uv;
			Vector2[] 		mUv2 			= mesh.uv2;
#if UNITY_5
			Vector2[] 		mUv3 			= mesh.uv3;
			Vector2[] 		mUv4 			= mesh.uv4;
#endif

			bool bVertices 					= !mVertices.NullOrEmpty();
			bool bBoneWeights 				= !mBoneWeights.NullOrEmpty();
			bool bColors 					= !mColors.NullOrEmpty();
			bool bColors32 					= !mColors32.NullOrEmpty();
			bool bNormals 					= !mNormals.NullOrEmpty();
			bool bTangents 					= !mTangents.NullOrEmpty();
			bool bUv 						= !mUv.NullOrEmpty();
			bool bUv2 						= !mUv2.NullOrEmpty();
#if UNITY_5
			bool bUv3 						= !mUv3.NullOrEmpty();
			bool bUv4 						= !mUv4.NullOrEmpty();
#endif

			Vector3[] 		vertices 		= bVertices ? new Vector3[ vertexCount - removedCount] 			: null;
			BoneWeight[] 	boneWeights 	= bBoneWeights ? new BoneWeight[ vertexCount - removedCount] 	: null;
			Color[] 		colors 			= bColors ? new Color[ vertexCount - removedCount] 				: null;
			Color32[] 		colors32 		= bColors32 ? new Color32[ vertexCount - removedCount] 			: null;
			Vector3[] 		normals 		= bNormals ? new Vector3[ vertexCount - removedCount] 			: null;
			Vector4[] 		tangents 		= bTangents ? new Vector4[ vertexCount - removedCount] 			: null;
			Vector2[] 		uv 				= bUv ? new Vector2[ vertexCount - removedCount] 				: null;
			Vector2[] 		uv2 			= bUv2 ? new Vector2[ vertexCount - removedCount] 				: null;
#if UNITY_5
			Vector2[] 		uv3 			= bUv3 ? new Vector2[ vertexCount - removedCount] 				: null;
			Vector2[] 		uv4 			= bUv4 ? new Vector2[ vertexCount - removedCount] 				: null;
#endif

			int index = 0;
			int removeIndex = 0;

			for(int i = 0; i < vertexCount; i++)
			{
				if( removeIndex < removedCount && i == removed[removeIndex] )
				{
					removeIndex++;
					continue;
				}

				if( bVertices ) 	vertices[index] 	= mVertices[i];
				if( bBoneWeights ) 	boneWeights[index] 	= mBoneWeights[i];
				if( bColors ) 		colors[index] 		= mColors[i];
				if( bColors32 ) 	colors32[index] 	= mColors32[i];
				if( bNormals ) 		normals[index] 		= mNormals[i];
				if( bTangents ) 	tangents[index] 	= mTangents[i];
				if( bUv ) 			uv[index] 			= mUv[i];
				if( bUv2 ) 			uv2[index] 			= mUv2[i];
#if UNITY_5
				if( bUv3 ) 			uv3[index] 			= mUv3[i];
				if( bUv4 ) 			uv4[index] 			= mUv4[i];
#endif

				index++;
			}

			mesh.vertices 		= vertices;
			mesh.boneWeights 	= boneWeights;
			mesh.colors 		= colors;
			mesh.colors32 		= colors32;
			mesh.normals 		= normals;
			mesh.tangents 		= tangents;
			mesh.uv 			= uv;
			mesh.uv2 			= uv2;
#if UNITY_5
			mesh.uv3 			= uv3;
			mesh.uv4 			= uv4;
#endif
		}
#endregion

#region Create

		public static void MakeFaceSelectionMesh(ref Mesh mesh, Vector3[] vertices)
		{
			int vl = vertices.Length;

			mesh.Clear();
			mesh.vertices = vertices;
			int[] triangles = new int[vl];
			for(int i = 0; i < triangles.Length; i++) triangles[i] = i;
			mesh.triangles = triangles;
			Color32[] colors = new Color32[vl];
			for(int i = 0; i < vl; i++)	colors[i] = qe_Constant.FaceSelectionColor;
			mesh.colors32 = colors;
			mesh.normals = vertices;
			mesh.uv = new Vector2[vl]; 
		}

		public static void MakeEdgeSelectionMesh(ref Mesh mesh, Vector3[] vertices, Vector3[] selected)
		{

			int vl = vertices.Length;
			int sl = selected.Length;

			mesh.Clear();

			Vector3[] v = new Vector3[vl + sl];
			
			System.Array.Copy(vertices, 0, v, 0, vl);
			System.Array.Copy(selected, 0, v, vl, sl);

			Color32[] colors = new Color32[vl+sl];

			for(int i = 0; i < vl; i++)
				colors[i] = Color.white;

			for(int i = vl; i < vl + sl; i++)
				colors[i] = Color.green;

			mesh.vertices = v;
			mesh.normals = v;
			mesh.colors32 = colors;

			int[] triangles = new int[vl + sl];
			for(int i = 0; i < triangles.Length; i++) triangles[i] = i;

			mesh.subMeshCount = 1;
			mesh.SetIndices(triangles, MeshTopology.Lines, 0);
		}

		public static void MakeVertexSelectionMesh(ref Mesh mesh, Vector3[] vertices, Vector3[] selected)
		{
			int vl = vertices.Length;
			int sl = selected.Length;

			Vector3[] v = new Vector3[vl + sl];
			System.Array.Copy(vertices, 0, v, 0, vl);
			System.Array.Copy(selected, 0, v, vl, sl);

			Vector3[] 	t_billboards 		= new Vector3[v.Length*4];
			Vector3[] 	t_nrm 				= new Vector3[v.Length*4];
			Vector2[] 	t_uvs 				= new Vector2[v.Length*4];
			Vector2[] 	t_uv2 				= new Vector2[v.Length*4];
			Color32[]   t_col 				= new Color32[v.Length*4];
			int[] 		t_tris 				= new int[v.Length*6];

			int n = 0;
			int t = 0;

			Vector3 up = Vector3.up;
			Vector3 right = Vector3.right;
			
			for(int i = 0; i < vl; i++)
			{
				t_billboards[t+0] = v[i];//-up-right;
				t_billboards[t+1] = v[i];//-up+right;
				t_billboards[t+2] = v[i];//+up-right;
				t_billboards[t+3] = v[i];//+up+right;

				t_uvs[t+0] = Vector3.zero;
				t_uvs[t+1] = Vector3.right;
				t_uvs[t+2] = Vector3.up;
				t_uvs[t+3] = Vector3.one;

				t_uv2[t+0] = -up-right;
				t_uv2[t+1] = -up+right;
				t_uv2[t+2] =  up-right;
				t_uv2[t+3] =  up+right;

				t_nrm[t+0] = Vector3.forward;
				t_nrm[t+1] = Vector3.forward;
				t_nrm[t+2] = Vector3.forward;
				t_nrm[t+3] = Vector3.forward;

				t_tris[n+0] = t+0;
				t_tris[n+1] = t+1;
				t_tris[n+2] = t+2;
				t_tris[n+3] = t+1;
				t_tris[n+4] = t+3;
				t_tris[n+5] = t+2;

				t_col[t+0] = (Color32) Color.white;
				t_col[t+1] = (Color32) Color.white;
				t_col[t+2] = (Color32) Color.white;
				t_col[t+3] = (Color32) Color.white;

				t+=4;
				n+=6;				
			}
			
			for(int i = vl; i < v.Length; i++)
			{
				t_billboards[t+0] = v[i];
				t_billboards[t+1] = v[i];
				t_billboards[t+2] = v[i];
				t_billboards[t+3] = v[i];

				t_uvs[t+0] = Vector3.zero;
				t_uvs[t+1] = Vector3.right;
				t_uvs[t+2] = Vector3.up;
				t_uvs[t+3] = Vector3.one;

				t_uv2[t+0] = -up-right;
				t_uv2[t+1] = -up+right;
				t_uv2[t+2] =  up-right;
				t_uv2[t+3] =  up+right;

				t_nrm[t+0] = Vector3.forward;
				t_nrm[t+1] = Vector3.forward;
				t_nrm[t+2] = Vector3.forward;
				t_nrm[t+3] = Vector3.forward;

				t_tris[n+0] = t+0;
				t_tris[n+1] = t+1;
				t_tris[n+2] = t+2;
				t_tris[n+3] = t+1;
				t_tris[n+4] = t+3;
				t_tris[n+5] = t+2;
	
				t_col[t+0] = (Color32) Color.green;
				t_col[t+1] = (Color32) Color.green;
				t_col[t+2] = (Color32) Color.green;
				t_col[t+3] = (Color32) Color.green;

				t_nrm[t].x = .1f;
				t_nrm[t+1].x = .1f;
				t_nrm[t+2].x = .1f;
				t_nrm[t+3].x = .1f;

				t+=4;
				n+=6;				
			}


			mesh.Clear();
			mesh.vertices = t_billboards;
			mesh.normals = t_nrm;
			mesh.uv = t_uvs;
			mesh.uv2 = t_uv2;
			mesh.colors32 = t_col;
			mesh.triangles = t_tris;
		}
#endregion
	}
}
